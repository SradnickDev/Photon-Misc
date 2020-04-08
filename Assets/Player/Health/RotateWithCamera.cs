using UnityEngine;

public class RotateWithCamera : MonoBehaviour
{
	private Transform m_camera;

	private void Awake()
	{
		m_camera = Camera.main.transform;
	}

	private void LateUpdate() => Rotate();

	private void Rotate()
	{
		if (!m_camera) return;

		transform.eulerAngles = new Vector3
		(
			m_camera.transform.eulerAngles.x,
			m_camera.transform.eulerAngles.y,
			0
		);
	}
}