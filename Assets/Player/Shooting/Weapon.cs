using Photon.Pun;
using UnityEngine;

public class Weapon : MonoBehaviourPun
{
	[SerializeField] private Bullet m_bullet = null;
	[SerializeField] private float m_fireRate = 0.25f;

	private float m_nextFire;

	private void Start()
	{
		m_nextFire = Time.time;
	}

	private void Update()
	{
		ManageInput();
	}

	private void ManageInput()
	{
		if (!photonView.IsMine) return;

		if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= m_nextFire)
		{
			var dir = transform.forward;
			photonView.RPC(nameof(FireBullet), RpcTarget.AllViaServer, dir);
			m_nextFire = Time.time + m_fireRate;
		}
	}

	[PunRPC]
	private void FireBullet(Vector3 direction)
	{
		var bullet = Instantiate(m_bullet, transform.position, Quaternion.identity);
		bullet.Setup(direction, this.gameObject);
	}
}