using Photon.Pun;
using UnityEngine;

/// <summary>
/// Using naive Interpolation approach, to synchronize IK Position and Rotation, to recreate the Movement on other Clients.
/// Synchronize values to handle Animator over Network.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class PlayerAnimSyncModel : MonoBehaviour, IPunObservable
{
	private readonly int SpeedHash = Animator.StringToHash("Movement");

	public Vector3 NetworkAimPos { get; private set; }

	[SerializeField] private PlayerMovementModel PlayerMovementModel = null;
	[SerializeField] private Transform RightShoulderIK = null;
	[SerializeField] private Animator Animator = null;

	private PhotonView m_photonView = null;

	private float m_fraction = 0.0f;
	private Vector3 m_updatedShoulderPos = new Vector3(0, 0, 0);
	private Vector3 m_lastShoulderPos = new Vector3(0, 0, 0);

	private Quaternion m_updatedShoulderRot = new Quaternion(0, 0, 0, 1);
	private Quaternion m_lastShoulderRot = new Quaternion(0, 0, 0, 1);

	private Quaternion m_updatedRot = new Quaternion(0, 0, 0, 1);
	private Quaternion m_lastRot = new Quaternion(0, 0, 0, 1);

	private void Awake()
	{
		if (RightShoulderIK == null)
		{
			Debug.LogWarning("Shoulder IK Transform is Missing, check out PlayerAnimSyncModel!");
		}

		m_photonView = GetComponent<PhotonView>();

		m_updatedShoulderPos = RightShoulderIK.localPosition;
		m_lastShoulderPos = RightShoulderIK.localPosition;

		m_updatedShoulderRot = RightShoulderIK.localRotation;
		m_lastShoulderRot = RightShoulderIK.localRotation;
	}

	private void Update()
	{
		if (m_photonView.IsMine) return;

		UpdateLocalRotation();
		UpdateShoulder();
	}

	/// <summary>
	/// Rotate towards the newest Rotation.
	/// </summary>
	private void UpdateLocalRotation()
	{
		transform.localRotation = Quaternion.RotateTowards(m_updatedRot, m_lastRot, 180);
	}

	/// <summary>
	/// Moves the Shoulder Rotation and Position to given Rotation and Position with fraction.
	/// </summary>
	private void UpdateShoulder()
	{
		m_fraction = m_fraction + Time.deltaTime * 9;
		RightShoulderIK.localPosition =
			Vector3.Lerp(m_updatedShoulderPos, m_lastShoulderPos, m_fraction);
		RightShoulderIK.localRotation =
			Quaternion.Lerp(m_updatedShoulderRot, m_lastShoulderRot, m_fraction);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(transform.localRotation);
			stream.SendNext(PlayerMovementModel.AimPoint);
			stream.SendNext(Animator.GetFloat(SpeedHash));
			stream.SendNext(RightShoulderIK.localPosition);
			stream.SendNext(RightShoulderIK.localRotation);
		}
		else
		{
			ReceiveLocalRotation(stream);
			NetworkAimPos = (Vector3) stream.ReceiveNext();
			ReceiveAnimationStates(stream);
			ReceiveShouldState(stream);
		}
	}

	/// <summary>
	/// Reading from photon stream to get values for Animator.
	/// </summary>
	/// <param name="stream"></param>
	private void ReceiveAnimationStates(PhotonStream stream)
	{
		Animator.SetFloat(SpeedHash, (float) stream.ReceiveNext());
	}

	/// <summary>
	/// Reading from photon stream to get values and set local rotation.
	/// </summary>
	/// <param name="stream"></param>
	private void ReceiveLocalRotation(PhotonStream stream)
	{
		m_lastRot = (Quaternion) stream.ReceiveNext();
		m_updatedRot = transform.localRotation;
	}

	/// <summary>
	/// Reading from photon stream to get values and set shoulder position and rotation
	/// </summary>
	/// <param name="stream"></param>
	private void ReceiveShouldState(PhotonStream stream)
	{
		m_lastShoulderPos = (Vector3) stream.ReceiveNext();
		m_updatedShoulderPos = RightShoulderIK.localPosition;

		m_lastShoulderRot = (Quaternion) stream.ReceiveNext();
		m_updatedShoulderRot = RightShoulderIK.localRotation;

		m_fraction = 0;
	}
}