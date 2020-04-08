using Photon.Pun;
using UnityEngine;

public class StateBuffer : MonoBehaviourPun, IPunObservable
{
	[SerializeField] private float InterpolationBackTime = 0.1f;
	[SerializeField] private float ExtrapolationLimit = 0.5f;

	private struct State
	{
		public double Timestamp;
		public Vector3 Position;
		public Quaternion Rotation;

		public State(double timestamp, Vector3 position, Quaternion rotation)
		{
			Timestamp = timestamp;
			Position = position;
			Rotation = rotation;
		}
	}

	private State[] m_buffer = new State[30];
	private int m_stateCount = 0;

	private void FixedUpdate()
	{
		if (photonView.IsMine || m_stateCount == 0) return;

		var currentTime = PhotonNetwork.Time;
		var interpolationTime = currentTime - InterpolationBackTime;

		if (m_buffer[0].Timestamp > interpolationTime)
		{
			Interpolation(interpolationTime);
		}
		else
		{
			Extrapolation(interpolationTime);
		}
	}

	private void Interpolation(double interpolationTime)
	{
		for (var i = 0; i < m_stateCount; i++)
		{
			//closest state that matches network Time or use oldest state
			if (m_buffer[i].Timestamp <= interpolationTime || i == m_stateCount - 1)
			{
				//closest to Network
				var lhs = m_buffer[i];

				//one newer
				var rhs = m_buffer[Mathf.Max(i - 1, 0)];

				//time between
				var length = rhs.Timestamp - lhs.Timestamp;

				var t = 0.0f;
				if (length > 0.0001f)
				{
					t = (float) ((interpolationTime - lhs.Timestamp) / length);
				}

				transform.position = Vector3.Lerp(lhs.Position, rhs.Position, t);
				transform.rotation = Quaternion.Slerp(lhs.Rotation, rhs.Rotation, t);
				break;
			}
		}
	}

	private void Extrapolation(double interpolationTime)
	{
		var latestState = m_buffer[0];

		//TODO implement proper extrapolation
		var extrapolationLength = (float) (interpolationTime - latestState.Timestamp);
		if (extrapolationLength < ExtrapolationLimit)
		{
			transform.position = latestState.Position;
			transform.rotation = latestState.Rotation;
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else
		{
			var pos = (Vector3) stream.ReceiveNext();
			var rot = (Quaternion) stream.ReceiveNext();

			var newState = new State(info.SentServerTime, pos, rot);

			AddState(newState);

			for (var i = 0; i < m_stateCount - 1; i++)
			{
				if (m_buffer[i].Timestamp < m_buffer[i + 1].Timestamp)
				{
					//Debug.Log("State inconsistent");
				}
			}
		}
	}

	/// <summary>
	/// Shift the states to the right.Storing newst at 0.
	/// </summary>
	/// <param name="state"></param>
	private void AddState(State state)
	{
		for (var i = m_buffer.Length - 1; i > 0; i--)
		{
			m_buffer[i] = m_buffer[i - 1];
		}

		//Newest State
		m_buffer[0] = state;

		m_stateCount = Mathf.Min(m_stateCount + 1, m_buffer.Length);
	}
}