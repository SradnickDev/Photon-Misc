using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
	[SerializeField] private Rigidbody m_rigidbody = null;
	[SerializeField] private float m_force = 10;
	[SerializeField] private ForceMode m_forceMode = ForceMode.Force;
	[SerializeField] private int m_damage = 10;
	private GameObject m_owner;

	public void Setup(Vector3 dir, GameObject from)
	{
		m_owner = from;
		transform.forward = dir;
		m_rigidbody.AddForce(dir * m_force, m_forceMode);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == m_owner) return;

		DealDamage(other);
		Destroy(this.gameObject);
	}

	private void DealDamage(Component other)
	{
		var damageable = other.GetComponent<IDamageable>();

		damageable?.ApplyDamage(m_damage);
	}
}