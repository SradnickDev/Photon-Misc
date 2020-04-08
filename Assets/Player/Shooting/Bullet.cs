using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
	[SerializeField] private Rigidbody m_rigidbody = null;
	[SerializeField] private float m_force = 10;
	[SerializeField] private ForceMode m_forceMode = ForceMode.Force;
	[SerializeField] private int m_damage = 10;
	[SerializeField] private float m_lifeTime = 2;
	private PhotonView m_owner;

	public void Setup(Vector3 dir, PhotonView from)
	{
		Destroy(gameObject, m_lifeTime);
		m_owner = from;
		transform.forward = dir;
		m_rigidbody.AddForce(dir * m_force, m_forceMode);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == m_owner.gameObject) return;

		if (m_owner.IsMine)
		{
			DealDamage(other);
		}

		Destroy(gameObject);
	}

	private void DealDamage(Component other)
	{
		var damageable = other.GetComponent<IDamageable>();
		damageable?.ApplyDamage(m_damage);
	}
}