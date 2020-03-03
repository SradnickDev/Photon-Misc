using Photon.Pun;
using UnityEngine;

namespace BulkSync.Unit
{
	[RequireComponent(typeof(UnitMovement))]
	[RequireComponent(typeof(UnitSync))]
	public class UnitBase : MonoBehaviour, IRecyclable
	{
		private PhotonView m_view;
		private UnitManager m_unitManager;
		private UnitSync m_sync;
		private UnitMovement m_movement;

		public void Setup(PhotonView view, UnitManager unitManager)
		{
			m_view = view;
			m_sync = GetComponent<UnitSync>();
			m_movement = GetComponent<UnitMovement>();
			m_unitManager = unitManager;
		}

		public void SetData(Vector3 position, Vector3 rotation, bool interpolate)
		{
			m_sync.AddState(position, rotation, interpolate);
		}

		private void Update()
		{
			if (m_view.IsMine)
			{
				m_movement.Move();
			}
			else
			{
				m_sync.Synchronize();
			}
		}

		public void OnDeath()
		{
			if (m_view.IsMine)
			{
				gameObject.SetActive(false);
			}
			else
			{
				m_unitManager.SendStateChange(this);
			}
		}

		private void OnDisable()
		{
			Recycle();
		}

		public void Recycle()
		{
			m_unitManager.Recycle(this);
		}
	}
}