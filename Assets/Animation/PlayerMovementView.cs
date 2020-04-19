using Photon.Pun;
using UnityEngine;

public class PlayerMovementView : MonoBehaviour
{
	[SerializeField] private PhotonView m_view;
	[SerializeField] private PlayerMovementModel PlayerMovementModel = null;
	[SerializeField] private PlayerAnimSyncModel PlayerAnimSyncModel = null;
	[SerializeField] private Animator Animator = null;

#region IK

	[Header("IK Weight")] 
	[SerializeField] private float SmoothTime = 1;
	[SerializeField] private float UpdateLookPosThreshold = 0.5f;
	[SerializeField] private float LookWeight = 1;
	[SerializeField] private float BodyWeight = .5f;
	[SerializeField] private float HeadWeight = .6f;
	[SerializeField] private float ClampWeight = 1;
	[SerializeField] private float RightHandWeight = 1;
	[SerializeField] private float LeftHandWeight = 1;

	[Header("Hand Rotation")]
	[SerializeField]
	private Vector3 LeftHandRotation = new Vector3(0, 0, 0);

	[SerializeField] private Vector3 RightHandRotation = new Vector3(0, 0, 0);
	private Transform m_leftHandBone;
	private Transform m_rightHandBone;

	[Header("IK Target")] [SerializeField] private Transform RightHandTarget = null;
	[SerializeField] private Transform RightElbowTarget = null;
	[SerializeField] private Transform LeftHandTarget = null;
	[SerializeField] private Transform LeftElbowTarget = null;

	private Vector3 m_lookAtPosition = new Vector3(0, 0, 0);
	private Vector3 m_ikLookPos = new Vector3(0, 0, 0);
	private Vector3 m_targetPos = new Vector3(0, 0, 0);
	private Vector3 m_iKVelocity = new Vector3(0, 0, 0);
	private Vector3 m_aimAtPosition = new Vector3(0, 0, 0);

#endregion

#region AnimatorProperties

	private readonly int MoveHash = Animator.StringToHash("Movement");
	private readonly int JumpHash = Animator.StringToHash("Jump");
	private readonly int HitHash = Animator.StringToHash("Hit");
	private readonly int FireHash = Animator.StringToHash("Fire");

#endregion

	private void Start()
	{
		SetupIK();
	}

	/// <summary>
	/// Get Bones.
	/// </summary>
	private void SetupIK()
	{
		m_leftHandBone = Animator.GetBoneTransform(HumanBodyBones.LeftHand);
		m_rightHandBone = Animator.GetBoneTransform(HumanBodyBones.RightHand);
	}

	private void Update()
	{
		if (!m_view.IsMine) return;
		SetMoveAnim();
	}

	/// <summary>
	/// Set value to Animator.
	/// </summary>
	private void SetMoveAnim()
	{
		var moving = PlayerMovementModel.IsMoving;

		if (moving && PlayerMovementModel.IsGrounded)
		{
			if (PlayerMovementModel.MovingForward)
			{
				Animator.SetFloat(MoveHash, 1);
			}
			else
			{
				Animator.SetFloat(MoveHash, -1);
			}
		}
		else
		{
			Animator.SetFloat(MoveHash, 0);
		}
	}

#region IK

	/// <summary>
	/// Define Bone Weight, position and rotation.
	/// </summary>
	/// <param name="layerIndex"></param>
	private void OnAnimatorIK(int layerIndex)
	{
		SetHandWeight();

		SetHandPosition();

		SetHandRotationWeight();

		SetHandRotation();

		SetElbowWeight();

		SetElbowPosition();


		Animator.SetLookAtWeight(LookWeight, BodyWeight, HeadWeight, HeadWeight, ClampWeight);

		SetLookAtPosition();
	}

	private void SetLookAtPosition()
	{
		m_aimAtPosition = m_view.IsMine
			? PlayerMovementModel.AimPoint
			: PlayerAnimSyncModel.NetworkAimPos;

		this.m_lookAtPosition = m_aimAtPosition;

		m_lookAtPosition.z = transform.position.z;

		var dist = Vector3.Distance(m_lookAtPosition, transform.position);

		if (dist > UpdateLookPosThreshold)
		{
			m_targetPos = m_lookAtPosition;
		}

		m_ikLookPos =
			Vector3.SmoothDamp(m_ikLookPos, m_targetPos, ref m_iKVelocity, SmoothTime);

		Animator.SetLookAtPosition(m_ikLookPos);
	}

	private void SetHandWeight()
	{
		Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, RightHandWeight);
		Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, LeftHandWeight);
	}

	private void SetHandPosition()
	{
		Animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
		Animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
	}

	private void SetHandRotationWeight()
	{
		Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, RightHandWeight);
		Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, LeftHandWeight);
	}

	private void SetHandRotation()
	{
		m_rightHandBone.LookAt(m_aimAtPosition, PlayerMovementModel.transform.up);
		m_leftHandBone.LookAt(m_aimAtPosition, PlayerMovementModel.transform.up);

		Animator.SetIKRotation(AvatarIKGoal.LeftHand,
							   m_leftHandBone.rotation * Quaternion.Euler(LeftHandRotation));
		Animator.SetIKRotation(AvatarIKGoal.RightHand,
							   m_rightHandBone.rotation * Quaternion.Euler(RightHandRotation));
	}

	private void SetElbowWeight()
	{
		Animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, RightHandWeight);
		Animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, LeftHandWeight);
	}

	private void SetElbowPosition()
	{
		Animator.SetIKHintPosition(AvatarIKHint.RightElbow, RightElbowTarget.position);
		Animator.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftElbowTarget.position);
	}

#endregion
}