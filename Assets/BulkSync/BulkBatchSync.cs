using System;
using System.Collections.Generic;
using BulkSync.Unit;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utilities;

#pragma warning disable 0649
namespace BulkSync
{
	public class BulkBatchSync : MonoBehaviour, IOnEventCallback
	{
		public const int BatchSize = 50;
		public const byte UpdateCode = 1;
		public const byte StateUpdate = 2;

		[Range(1, 20)]
		[SerializeField] int m_maxSendRate = 20;

		[Range(0, 1)]
		[SerializeField] private float m_utilizeSendRatePercentage = 0.5f;

		[SerializeField] private bool m_stopProcess = false;
		[SerializeField] private int m_batchProcressRate = 0;
		[SerializeField] private uint m_receiveCount = 0;
		[SerializeField] private uint m_sendCount = 0;
		[SerializeField] private int m_queuedBatches = 0;
		[SerializeField] private int m_batchesInRow = 0;

		[Header("Compression Infos")]
		[SerializeField] private Vector3 m_minPosition;

		[SerializeField] private Vector3 m_maxPosition;
		[SerializeField] private Vector3 m_minAngle = new Vector3(0, 0, 0);
		[SerializeField] private Vector3 m_maxAngle = new Vector3(360, 360, 360);

		private PhotonView m_view;
		private float m_timeTillNextFlush;
		private Queue<byte[]> m_outgoingData = new Queue<byte[]>();
		private Queue<BulkBatch> m_incomingBatches = new Queue<BulkBatch>();
		private Queue<int> m_stateChanges = new Queue<int>();

		public event Action<int, bool> GameObjectStateChanged;

#region Setup

		public void OnEnable()
		{
			PhotonNetwork.AddCallbackTarget(this);
		}

		public void OnDisable()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}

		private void Awake()
		{
			m_view = GetComponent<PhotonView>();
		}

		private void Start()
		{
			m_timeTillNextFlush = Time.time;
		}

#endregion

		public void CreateBatches(List<UnitBase> units)
		{
			if (m_stopProcess) return;

			foreach (var stateChange in m_stateChanges)
			{
				units[stateChange].gameObject.SetActive(false);
			}

			m_stateChanges = new Queue<int>();

			if (Time.time > m_timeTillNextFlush)
			{
				var count = 0;
				byte currentId = 1;

				var batch = new BulkBatch
				{
					Id = currentId,
					Position = new float[BatchSize],
					Rotation = new float[BatchSize],
				};

				foreach (var unit in units)
				{
					var packedPos = 0f;
					var packedRot = 0f;

					//set data only for active objects
					if (unit.gameObject.activeInHierarchy)
					{
						var position = unit.transform.position;
						var rotation = unit.transform.rotation.eulerAngles;
						packedPos = Compression.PackVector3(position, m_minPosition, m_maxPosition);
						packedRot = Compression.PackVector3(rotation, m_minAngle, m_maxAngle);
					}

					batch.Position[count] = packedPos;
					batch.Rotation[count] = packedRot;

					if (count == BatchSize - 1 || count == units.Count - 1)
					{
						var arr = batch.Serialize();
						m_outgoingData.Enqueue(arr);
						m_queuedBatches++;

						currentId++;
						count = -1;

						batch = new BulkBatch
						{
							Id = currentId,
							Position = new float[BatchSize],
							Rotation = new float[BatchSize],
						};
					}

					count++;
				}

				m_batchesInRow = units.Count / BatchSize;
				m_batchProcressRate = Mathf.FloorToInt(m_maxSendRate * m_utilizeSendRatePercentage);
				m_batchProcressRate -= m_batchesInRow;
				m_timeTillNextFlush = (1f / (Mathf.Max(m_batchProcressRate, 0))) + Time.time;
			}
		}

		public void Sync(List<UnitBase> units)
		{
			if (m_incomingBatches.Count > 0)
			{
				var batch = m_incomingBatches.Dequeue();
				var endIdx = Mathf.Max(batch.Id * BatchSize, BatchSize);
				var startIdx = endIdx - BatchSize;

				for (var i = startIdx; i < endIdx; i++)
				{
					if (i <= units.Count - 1)
					{
						var unit = units[i];
						var packedPos = batch.Position[i - startIdx];
						var packedRot = batch.Rotation[i - startIdx];

						//invoke that an gbj has changed to inactive
						if (packedPos == 0 && unit.gameObject.activeInHierarchy)
						{
							GameObjectStateChanged?.Invoke(i, false);
							continue;
						}

						var interpolate = true;

						//invoke that an gbj has change to active
						if (packedPos != 0 && !unit.gameObject.activeInHierarchy)
						{
							GameObjectStateChanged?.Invoke(i, true);
							interpolate = false;
						}

						var position = Compression.UnpackVector3(packedPos, m_minPosition, m_maxPosition);
						var rotation = Compression.UnpackVector3(packedRot, m_minAngle, m_maxAngle);
						unit.SetData(position, rotation, interpolate);
					}
				}
			}
		}

		private void Update()
		{
			if (!m_view.IsMine) return;
			if (m_stopProcess) return;
			PhotonNetwork.SendRate = m_maxSendRate;
			PhotonNetwork.SerializationRate = m_maxSendRate;

			SendBatchUpdate();
		}

		private void SendBatchUpdate()
		{
			if (m_outgoingData.Count > 0)
			{
				var byteArr = m_outgoingData.Dequeue();
				var raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.Others};
				var sendOptions = new SendOptions {Reliability = true};
				if (PhotonNetwork.RaiseEvent(UpdateCode, byteArr, raiseEventOptions,
											 sendOptions))
				{
					m_sendCount++;
				}
				else
				{
					Debug.LogWarning("SendRate Limit reached!!");
				}
			}
		}

		public void SendStateChange(int index)
		{
			var raiseEventOptions = new RaiseEventOptions
				{TargetActors = new[] {PhotonNetwork.MasterClient.ActorNumber}};
			var sendOptions = new SendOptions {Reliability = true};
			PhotonNetwork.RaiseEvent(StateUpdate, index, raiseEventOptions, sendOptions);
		}

		public void OnEvent(EventData photonEvent)
		{
			switch (photonEvent.Code)
			{
				case UpdateCode:
					EnqueueReceivedBatch(photonEvent);
					break;
				case StateUpdate:
					QueueStateChanges(photonEvent);
					break;
			}
		}

		private void QueueStateChanges(EventData photonEvent)
		{
			var idx = (int) photonEvent.CustomData;
			m_stateChanges.Enqueue(idx);
		}

		private void EnqueueReceivedBatch(EventData photonEvent)
		{
			var data = (byte[]) photonEvent.CustomData;
			var batch = BulkUtilities.Deserialize(data);
			m_incomingBatches.Enqueue(batch);

			m_receiveCount++;
		}
	}
}
#pragma warning restore 0649