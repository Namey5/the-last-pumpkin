using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	[Header ("Components")]
	[SerializeField] private CharacterController m_CharacterController;
	[SerializeField] private Animator m_Animator;

	[Header ("UI")]
	[SerializeField] private Image m_Crosshair;
	[SerializeField] private float m_MaxCrosshairDistance = 2f;

	[Header ("Movement")]
	[SerializeField] private float m_MovementSpeed = 3f;
	[SerializeField] private float m_JumpForce = 3f;
	[SerializeField] private float m_RotationSpeed = 10f;

	[Header ("Equipment")]
	[SerializeField] private GameObject m_EquipRoot;
	[SerializeField] private InventoryItem[] m_InventoryItems;

	private int m_Health = 100;

	private Vector3 m_Velocity;
	private Quaternion m_Rotation;
	private CollisionFlags m_LastCollision;

	private bool m_IsAiming = false;

	public CharacterController CharacterController => m_CharacterController;
	public Animator Animator => m_Animator;

	public Vector3 Velocity => m_Velocity;
	public Quaternion Rotation => m_Rotation;
	public bool IsGrounded => m_CharacterController.isGrounded;

	public bool IsAiming => m_IsAiming;

	private void Update ()
	{
		Quaternion _CameraDir = Quaternion.AngleAxis (GameManager.Instance.MainCamera.transform.eulerAngles.y, Vector3.up);

		Vector3 _LookDirection = new Vector3 (Input.GetAxis ("AimX"), 0f, Input.GetAxis ("AimY"));
		_LookDirection = _CameraDir * _LookDirection;

		float _CrosshairDist = 0f;

		bool _AimingWithController = _LookDirection.sqrMagnitude >= 0.1f;
		m_IsAiming = _AimingWithController || Input.GetButton ("Fire2");
		if (m_IsAiming)
		{
			m_Velocity.x = 0f;
			m_Velocity.z = 0f;

			m_Animator.SetFloat ("Speed", 0f);

			if (!_AimingWithController)
			{
				Ray _MouseRay = GameManager.Instance.MainCamera.ScreenPointToRay (Input.mousePosition);
				Plane _Plane = new Plane (Vector3.up, transform.position);
				if (_Plane.Raycast (_MouseRay, out float _PlaneDist))
				{
					Vector3 _Hit = _MouseRay.GetPoint (_PlaneDist);
					_LookDirection = (_Hit - transform.position) / m_MaxCrosshairDistance;
				}
			}

			_CrosshairDist = Mathf.Clamp01 (_LookDirection.magnitude);

			_LookDirection.Normalize ();
			m_Rotation = Quaternion.LookRotation (_LookDirection);
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
		}

		m_Crosshair.transform.localPosition = new Vector3 (0f, _CrosshairDist * m_MaxCrosshairDistance, 0f);
		m_Crosshair.color = new Color (1f, 1f, 1f, Mathf.Clamp01 (_CrosshairDist * 2f - 0.5f));

		m_Rotation = Quaternion.Slerp (m_Rotation, m_Rotation, 1f - Mathf.Exp (-m_RotationSpeed * Time.deltaTime));
		transform.rotation = m_Rotation;

		m_Velocity += Physics.gravity * Time.deltaTime;

		m_LastCollision = m_CharacterController.Move (m_Velocity * Time.deltaTime);

		m_Animator.SetBool ("IsGrounded", m_CharacterController.isGrounded);
		m_Animator.SetBool ("IsAiming", IsAiming);

		if (Input.GetButton ("Fire1") || Input.GetAxis ("Fire1") > 0.4f)
		{
			UseItem ();
		}
	}

	private void UseItem ()
	{
		m_Animator.ResetTrigger ("Shoot");
		m_Animator.SetTrigger ("Shoot");

		Vector3 _AimDir = m_Rotation * Vector3.forward;
		if (Physics.Raycast (transform.position, _AimDir, out RaycastHit _Hit, 100f))
		{
			if (_Hit.collider.TryGetComponent (out Zombie _Zombie))
			{
				_Zombie.Damage (_AimDir, 50);
			}
		}
	}

	public void GiveItem (ItemDefinition a_Item)
	{

	}
}
