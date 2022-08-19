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
		Vector3 _Input = new Vector3 (Input.GetAxis ("Horizontal"), 0f, Input.GetAxis ("Vertical"));
		Vector3 _Movement = Quaternion.AngleAxis (m_MainCamera.transform.eulerAngles.y, Vector3.up) * _Input;
		_Movement *= m_MovementSpeed;

		m_Velocity.x = _Movement.x;
		m_Velocity.z = _Movement.z;

		if (m_CharacterController.isGrounded)
		{
			if (Input.GetButtonDown ("Jump"))
			{
				m_Velocity.y = m_JumpForce;
			}
		}

		m_Velocity += Physics.gravity * Time.deltaTime;

		m_LastCollision = m_CharacterController.Move (m_Velocity * Time.deltaTime);

		if (_Movement.sqrMagnitude > 0.01f)
		{
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (_Movement), 1f - Mathf.Exp (-m_RotationSpeed * Time.deltaTime));
		}

		// Animation
		m_Animator.SetFloat ("Speed", Mathf.Clamp01 (_Movement.magnitude / m_MovementSpeed));
		m_Animator.SetBool ("IsGrounded", m_CharacterController.isGrounded);
	}
}
