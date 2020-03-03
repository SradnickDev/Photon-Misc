using Photon.Pun;
using UnityEngine;

#pragma warning disable 0649
namespace BulkSync.Unit
{
	[RequireComponent(typeof(PhotonView))]
	[RequireComponent(typeof(BulkBatchSync))]
	public class UnitManager : MonoBehaviour
	{

		[SerializeField,Range(0,200)] private int m_unitAmount = 50;
		[SerializeField] private UnitBase m_unit;
		private PhotonView m_view;
		private BulkBatchSync m_bulkBatchSync;
		private ObjectPool<UnitBase> m_pool = new ObjectPool<UnitBase>();

		private void Awake()
		{
			m_view = GetComponent<PhotonView>();
			m_bulkBatchSync = GetComponent<BulkBatchSync>();
		}

		private void Start()
		{
			TransferOwnership();

			m_bulkBatchSync.GameObjectStateChanged += OnUnitStateChanged;
			CreateUnit(m_unitAmount);
		}

		private void TransferOwnership()
		{
			if (PhotonNetwork.IsMasterClient)
			{
				m_view.TransferOwnership(PhotonNetwork.MasterClient);
			}
		}

		private void CreateUnit(int count)
		{
			m_pool.Initialize(count, m_unit, this.transform);
			foreach (var unit in m_pool.Pool)
			{
				unit.GetComponent<UnitBase>().Setup(m_view, this);
			}
		}

		public void ActivateUnit(Vector3 spawnPos)
		{
			if (m_pool.TryPop(out var unit))
			{
				unit.transform.position = spawnPos;
				unit.gameObject.SetActive(true);
			}
		}

		private void Update()
		{
			//TODO simulate the movement but do not apply it, also the masterclient should receive the batches for better results between clients
			if (m_view.IsMine)
			{
				m_bulkBatchSync.CreateBatches(m_pool.Pool);
			}

			else
			{
				m_bulkBatchSync.Sync(m_pool.Pool);
			}
		}

		private void OnUnitStateChanged(int index, bool active)
		{
			m_pool.Pool[index].gameObject.SetActive(active);
		}

		public void SendStateChange(UnitBase unit)
		{
			if (!m_view.IsMine)
			{
				var idx = m_pool.Pool.FindIndex(x => x == unit);
				m_bulkBatchSync.SendStateChange(idx);
			}
		}

		public void Recycle(UnitBase unit)
		{
			m_pool.Push(unit);
		}

		private void OnDisable()
		{
			m_bulkBatchSync.GameObjectStateChanged -= OnUnitStateChanged;
		}
	}
}
#pragma warning restore 0649