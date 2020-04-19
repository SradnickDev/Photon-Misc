using System;
using Photon.Pun;
using UnityEngine;

public class PlayerMovementModel : MonoBehaviourPun
{
	[Header("Movement")] public Camera Camera;
	[SerializeField] private float MovementSpeed = 10;
	[SerializeField] private float RotationSpeed = 200;

	[Header("Collision")] public LayerMask GroundLayer;
	
	[SerializeField] private float GroundDistance = 1.5f;
	[Header("IK Shoulder")] [SerializeField] private Transform PlayerModel = null;

	[SerializeField] private Rigidbody Rigidbody;
	[SerializeField] private Transform RightShoulderBone = null;
	[SerializeField] private Transform RightShoulderIK = null;

	public Vector3 AimPoint { get; private set; }
	public bool MovingForward { get; private set; }
	public bool IsMoving { get; set; }
	public bool IsGrounded { get; set; }

	private Vector3 m_aimDampingVelocity = new Vector3(0, 0, 0);
	private GameObject m_ikHelper = null;
	private Vector3 m_previousPosition;
	private Vector3 m_groundNormal;
	private float m_rotationVelocity;

	private void Start()
	{
		SetupIK();
	}
	/// <summary>Determine model(mesh) rotation based on Model rotation and mouse position.s</summary>
	public void Facing()
	{
		//Direction to Aim Position / Mouse
		var directionToLook = (transform.position - AimPoint).normalized;

		const float threshold = 0.05f;
		var left = Vector3.Dot(directionToLook, transform.right) > threshold;
		var right = Vector3.Dot(directionToLook, transform.right) < -threshold;

		var targetAngle = 0;

		if (left)
		{
			targetAngle = 180;
		}

		if (right)
		{
			targetAngle = 0;
		}


		var y = Mathf.SmoothDampAngle(PlayerModel.localEulerAngles.y, targetAngle, ref m_rotationVelocity, 0.05f);

		PlayerModel.transform.localRotation = Quaternion.Euler(PlayerModel.transform.localRotation.x, y,
															   PlayerModel.transform.localRotation.z);
	}
	
	private void SetupIK()
	{
		m_ikHelper = new GameObject {name = transform.root.name + "IK Helper"};
		m_ikHelper.transform.SetParent(transform);
	}
	
	private void Movement(Vector3 input)
	{
		var dir = Camera.transform.TransformDirection(input);

		//transform direction accordingly to the ground normal
		var projectOnPlane = Vector3.ProjectOnPlane(dir, m_groundNormal);

		projectOnPlane = Vector3.ClampMagnitude(projectOnPlane, 1);
		Rigidbody.MovePosition(Rigidbody.position + Time.fixedDeltaTime * MovementSpeed * projectOnPlane);
	}

	/// <summary>
	/// Rotates the Rigidbody
	/// Rotation through Player input.
	/// </summary>
	/// <param name="input">Axis input</param>
	private void Rotation(float input)
	{
		var rotationDelta = RotationSpeed * Time.fixedDeltaTime * new Vector3(0, 0, input);
		Rigidbody.MoveRotation(Rigidbody.rotation * Quaternion.Euler(rotationDelta));
	}
	
	private void CheckFacingDirection()
	{
		var fwd = PlayerModel.right;
		var moveDir = (transform.position - m_previousPosition).normalized;

		const float threshold = 0.025f;
		var forward = Vector3.Dot(fwd, moveDir) > threshold;
		var backward = Vector3.Dot(fwd, moveDir) < -threshold;

		if (forward)
		{
			MovingForward = true;
		}

		if (backward)
		{
			MovingForward = false;
		}

		m_previousPosition = Rigidbody.position;
	}

	private void Update()
	{
		HandleShoulderRotation();
		Aim();
		CheckFacingDirection();
		Facing();
	}

	/// <summary>
	/// Inverse Kinematic, Shoulder Rotation.
	/// </summary>
	public void HandleShoulderRotation()
	{
		RightShoulderIK.LookAt(AimPoint, transform.up);

		var rightShoulderPos = RightShoulderBone.TransformPoint(Vector3.zero);
		m_ikHelper.transform.position = rightShoulderPos;
		m_ikHelper.transform.parent = transform;

		RightShoulderIK.position = m_ikHelper.transform.position;
	}

	/// <summary>
	/// Convert a Aim Position according to the Mouse position, little delayed.
	/// </summary>
	public void Aim()
	{
		var mousePos = Input.mousePosition;
		var ray = Camera.ScreenPointToRay(mousePos);
		var diff = -ray.origin.z / ray.direction.z;
		var target = ray.origin + ray.direction * diff;
		AimPoint = Vector3.SmoothDamp(AimPoint, target, ref m_aimDampingVelocity, 0.05f);
	}
}