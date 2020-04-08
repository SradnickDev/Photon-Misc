using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviourPun
{
	[SerializeField] private float m_speed = 10;
	[SerializeField] private Rigidbody m_rigidbody = null;

	private Vector3 m_moveDirection;

	private void Start()
	{
		m_rigidbody = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (!photonView.IsMine) return;
		ManageInput();
		Move();
	}

	private void ManageInput()
	{
		var vertical = Input.GetAxis("Vertical");
		var horizontal = Input.GetAxis("Horizontal");

		var dir = new Vector3(horizontal, 0, vertical);
		if (dir.magnitude > 1)
		{
			dir.Normalize();
		}
		
		m_moveDirection = dir;
	}

	private void Move()
	{
		var nextPosition = m_rigidbody.position + m_moveDirection * (m_speed * Time.deltaTime);
		m_rigidbody.MovePosition(nextPosition);
	}
}