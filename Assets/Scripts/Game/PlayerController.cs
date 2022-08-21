using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	[Header ("Components")]
	[SerializeField] private CharacterController m_CharacterController;
	[SerializeField] private Animator m_Animator;

	[Header ("Movement")]
	[SerializeField] private float m_MovementSpeed = 3f;
	[SerializeField] private float m_JumpForce = 3f;
	[SerializeField] private float m_RotationSpeed = 10f;

	[Header ("HUD")]
	[SerializeField] private HUD m_HUD;
	[SerializeField] private Image m_Crosshair;
	[SerializeField] private float m_MaxCrosshairDistance = 2f;

	private Camera m_MainCamera;

	private Vector3 m_Velocity;
	private Quaternion m_Rotation;
	private CollisionFlags m_LastCollision;

	private int m_KillCount = 0;

	public Vector2 Velocity => m_Velocity;
	public Quaternion Rotation => m_Rotation;

	private void Start ()
	{
		m_MainCamera = Camera.main;
	}

	private void Update ()
	{
		Quaternion _CameraDir = Quaternion.AngleAxis (m_MainCamera.transform.eulerAngles.y, Vector3.up);

		Vector3 _LookDirection = new Vector3 (Input.GetAxis ("AimX"), 0f, Input.GetAxis ("AimY"));
		_LookDirection = _CameraDir * _LookDirection;

		bool _AimingWithController = _LookDirection.sqrMagnitude >= 0.1f;
		if (_AimingWithController || Input.GetButton ("Fire2"))
		{
			m_Velocity.x = 0f;
			m_Velocity.z = 0f;

			m_Animator.SetFloat ("Speed", 0f);
			m_Animator.SetBool ("IsAiming", true);

			if (!_AimingWithController)
			{
				Ray _MouseRay = m_MainCamera.ScreenPointToRay (Input.mousePosition);
				Plane _Plane = new Plane (Vector3.up, transform.position);
				if (_Plane.Raycast (_MouseRay, out float _PlaneDist))
				{
					Vector3 _Hit = _MouseRay.GetPoint (_PlaneDist);
					_LookDirection = (_Hit - transform.position) * 0.5f;
				}
			}

			float _CrosshairDist = Mathf.Clamp01 (_LookDirection.magnitude);
			m_Crosshair.transform.localPosition = new Vector3 (0f, _CrosshairDist * m_MaxCrosshairDistance, 0f);
			m_Crosshair.color = new Color (1f, 1f, 1f, Mathf.Clamp01 (_CrosshairDist * 2f - 0.5f));

			_LookDirection.Normalize ();
			m_Rotation = Quaternion.LookRotation (_LookDirection);

			if (Input.GetButtonDown ("Fire1"))
			{
				FireWeapon ();
			}
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
				m_Rotation = Quaternion.LookRotation (_Movement);
			}

			if (m_CharacterController.isGrounded)
			{
				if (Input.GetButtonDown ("Jump"))
				{
					m_Velocity.y = m_JumpForce;
				}
			}

			m_Animator.SetFloat ("Speed", Mathf.Clamp01 (_Movement.magnitude / m_MovementSpeed));
			m_Animator.SetBool ("IsAiming", false);

			m_Crosshair.color = new Color (1f, 1f, 1f, 0f);

			if (Input.GetButtonDown ("Fire1"))
			{
				MeleeAttack ();
			}
		}

		m_Rotation = Quaternion.Slerp (m_Rotation, m_Rotation, 1f - Mathf.Exp (-m_RotationSpeed * Time.deltaTime));
		transform.rotation = m_Rotation;

		m_Velocity += Physics.gravity * Time.deltaTime;

		m_LastCollision = m_CharacterController.Move (m_Velocity * Time.deltaTime);

		m_Animator.SetBool ("IsGrounded", m_CharacterController.isGrounded);
	}

	private void FireWeapon ()
	{
		m_Animator.ResetTrigger ("Shoot");
		m_Animator.SetTrigger ("Shoot");

		Vector3 _AimDir = m_Rotation * Vector3.forward;
		if (Physics.Raycast (transform.position, _AimDir, out RaycastHit _Hit, 100f))
		{
			Zombie _Zombie = _Hit.collider.GetComponent<Zombie> ();
			if (_Zombie != null)
			{
				if (!_Zombie.Damage (_AimDir, 50))
				{
					m_KillCount++;
					m_HUD.UpdateKillCount (m_KillCount);
				}
			}
		}
	}

	private void MeleeAttack ()
	{
		m_Animator.ResetTrigger ("MeleeAttack");
		m_Animator.SetTrigger ("MeleeAttack");
	}
}
