using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header ("Components")]
	[SerializeField] private CharacterController m_CharacterController;
	[SerializeField] private Animator m_Animator;

	[Header ("Movement")]
	[SerializeField] private float m_MovementSpeed = 5f;
	[SerializeField] private float m_JumpForce = 6f;
	[SerializeField] private float m_RotationSpeed = 10f;

	private Camera m_MainCamera;

	private Vector3 m_Velocity;
	private CollisionFlags m_LastCollision;

	private void Start ()
	{
		m_MainCamera = Camera.main;
	}

	private void Update ()
	{
		// Movement
		Quaternion _CameraDir = Quaternion.AngleAxis (m_MainCamera.transform.eulerAngles.y, Vector3.up);

		Quaternion _TargetRotation = transform.rotation;
		if (Input.GetButton ("Fire2"))
		{
			m_Velocity.x = 0f;
			m_Velocity.z = 0f;

			m_Animator.SetFloat ("Speed", 0f);

			Vector3 _Direction = new Vector3 (Input.GetAxis ("AimX"), 0f, Input.GetAxis ("AimY"));
			_Direction = _CameraDir * _Direction;

			if (_Direction.sqrMagnitude < 0.1f)
			{
				Ray _MouseRay = m_MainCamera.ScreenPointToRay (Input.mousePosition);
				Plane _Plane = new Plane (Vector3.up, transform.position);
				if (_Plane.Raycast (_MouseRay, out float _PlaneDist))
				{
					Vector3 _Hit = _MouseRay.GetPoint (_PlaneDist);
					_Direction = _Hit - transform.position;
				}
			}

			_Direction.Normalize ();
			_TargetRotation = Quaternion.LookRotation (_Direction);
		}
		else
		{
			Vector3 _Input = new Vector3 (Input.GetAxis ("Horizontal"), 0f, Input.GetAxis ("Vertical"));
			Vector3 _Movement = _CameraDir * _Input;
			_Movement *= m_MovementSpeed;

			m_Velocity.x = _Movement.x;
			m_Velocity.z = _Movement.z;

			if (_Movement.sqrMagnitude > 0.01f)
			{
				_TargetRotation = Quaternion.LookRotation (_Movement);
			}

			if (m_CharacterController.isGrounded)
			{
				if (Input.GetButtonDown ("Jump"))
				{
					m_Velocity.y = m_JumpForce;
				}
			}

			m_Animator.SetFloat ("Speed", Mathf.Clamp01 (_Movement.magnitude / m_MovementSpeed));
		}

		transform.rotation = Quaternion.Slerp (transform.rotation, _TargetRotation, 1f - Mathf.Exp (-m_RotationSpeed * Time.deltaTime));

		m_Velocity += Physics.gravity * Time.deltaTime;

		m_LastCollision = m_CharacterController.Move (m_Velocity * Time.deltaTime);

		// Animation
		m_Animator.SetBool ("IsGrounded", m_CharacterController.isGrounded);
	}
}
