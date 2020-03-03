using UnityEngine;

namespace BulkSync.Unit
{
	public class UnitSync : MonoBehaviour
	{
		private Vector3 m_newRot;
		private Vector3 m_latestCorrectPos;
		private Vector3 m_onUpdatePos;
		private float m_fraction;

		public void Awake()
		{
			m_latestCorrectPos = transform.position;
			m_onUpdatePos = transform.position;
		}

		public void Synchronize()
		{
			m_fraction = m_fraction + Time.deltaTime * 2;
			transform.localPosition =
				Vector3.LerpUnclamped(m_onUpdatePos, m_latestCorrectPos, m_fraction);
			transform.rotation =
				Quaternion.Slerp(transform.rotation, Quaternion.Euler(m_newRot),
								 2 * Time.deltaTime);
		}

		public void AddState(Vector3 pos, Vector3 rotation, bool interpolate = true)
		{
			m_latestCorrectPos = pos;
			m_onUpdatePos = transform.localPosition;
			m_fraction = 0;
			m_newRot = rotation;

			if (!interpolate)
			{
				transform.position = pos;
				transform.rotation = Quaternion.Euler(rotation);
				m_latestCorrectPos = pos;
				m_onUpdatePos = pos;
			}
		}
	}
}