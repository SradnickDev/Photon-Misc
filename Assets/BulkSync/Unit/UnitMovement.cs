using UnityEngine;
using Random = UnityEngine.Random;

namespace BulkSync.Unit
{
	public class UnitMovement : MonoBehaviour
	{
		[SerializeField] private float m_speed = 5;
		private Vector3 m_destination;
		private MovementBounds m_movementBounds;

		private void Start()
		{
			m_movementBounds = FindObjectOfType<MovementBounds>();
			SetRandomDestination();
		}

		public void Move()
		{
			var dir = (m_destination - transform.position).normalized;
			transform.position += dir * (Time.deltaTime * m_speed);
			transform.rotation =
				Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 150f);

			var distance = Vector3.Distance(transform.position, m_destination);
			if (distance <= 0.5f)
			{
				SetRandomDestination();
			}
		}

		private void SetRandomDestination()
		{
			m_destination = m_movementBounds.RandomPoint;
		}
	}
}