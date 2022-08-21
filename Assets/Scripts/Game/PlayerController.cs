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
	[SerializeField] private GameObject m_MuzzleFX;

	[Header ("Combat")]
	[SerializeField] private float m_FireRate = 0.75f;
	[SerializeField] private int m_GunDamage = 50;
	[SerializeField] private float m_MeleeRate = 1f;
	[SerializeField] private float m_MeleeRange = 2f;
	[SerializeField] private int m_MeleeDamage = 50;

	[Header ("Audio")]
	[SerializeField] private AudioSource m_AudioSource;
	[SerializeField] private AudioClip m_WalkClip;
	[SerializeField] private AudioClip m_FireClip;
	[SerializeField] private AudioClip m_MeleeClip;

	private Vector3 m_Velocity;
	private Quaternion m_Rotation;
	private CollisionFlags m_LastCollision;

	private float m_LastStep = float.MinValue;

	private int m_Health = 100;
	private int m_Ammo = 20;

	private bool m_IsAiming = false;
	private float m_LastShoot = float.MinValue;
	private float m_LastMelee = float.MinValue;

	public CharacterController CharacterController => m_CharacterController;
	public Animator Animator => m_Animator;

	public Vector3 Velocity => m_Velocity;
	public Quaternion Rotation => m_Rotation;
	public bool IsGrounded => m_CharacterController.isGrounded;

	public bool IsAiming => m_IsAiming;
	public bool IsAlive => m_Health > 0;

	private void Start ()
	{
		GameManager.Instance.HUD.UpdateAmmoCount (m_Ammo);
	}

	private void Update ()
	{
		if (!IsAlive)
		{
			return;
		}

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

			float _CurrentSpeed = _Movement.magnitude;
			if (_CurrentSpeed > 0.02f)
			{
				m_Rotation = Quaternion.LookRotation (_Movement);

				if (IsGrounded && (Time.time - m_LastStep) > Mathf.Lerp (0.4f, 0.8f, 1f - Mathf.Clamp01 (_CurrentSpeed / m_MovementSpeed)))
				{
					m_LastStep = Time.time;
					m_AudioSource.PlayOneShot (m_WalkClip);
				}

			}

			if (IsGrounded)
			{
				if (Input.GetButtonDown ("Jump"))
				{
					m_Velocity.y = m_JumpForce;
				}
			}

			m_Animator.SetFloat ("Speed", Mathf.Clamp01 (_CurrentSpeed / m_MovementSpeed));
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
			Attack ();
		}
	}

	private void Attack ()
	{
		Vector3 _AimDir = m_Rotation * Vector3.forward;
		if (IsAiming && m_Ammo > 0)
		{
			if ((Time.time - m_LastShoot) < m_FireRate)
			{
				return;
			}

			m_Animator.SetTrigger ("Shoot");
			m_AudioSource.PlayOneShot (m_FireClip);
			m_MuzzleFX.SetActive (true);

			if (Physics.Raycast (transform.position, _AimDir, out RaycastHit _Hit, 100f))
			{
				if (_Hit.collider.TryGetComponent (out Zombie _Zombie))
				{
					_Zombie.Damage (_AimDir, m_GunDamage);
				}
			}

			m_Ammo--;
			GameManager.Instance.HUD.UpdateAmmoCount (m_Ammo);

			m_LastShoot = Time.time;
		}
		else
		{
			if ((Time.time - m_LastMelee) < m_MeleeRate)
			{
				return;
			}

			m_Animator.SetTrigger ("MeleeAttack");
			m_AudioSource.PlayOneShot (m_MeleeClip);

			Collider[] _HitColliders = Physics.OverlapSphere (transform.position + _AimDir * m_MeleeRange * 0.5f, m_MeleeRange * 0.5f);
			if (_HitColliders != null && _HitColliders.Length > 0)
			{
				foreach (Collider _Hit in _HitColliders)
				{
					if (_Hit.TryGetComponent (out Zombie _Zombie))
					{
						_Zombie.Damage (_AimDir, m_MeleeDamage);
					}
				}
			}

			m_LastMelee = Time.time;
		}
	}

	public void Damage (int a_Damage)
	{
		m_Health -= a_Damage;
		GameManager.Instance.HUD.UpdatePlayerHealth (Mathf.Max (0, m_Health));

		if (m_Health <= 0)
		{
			//m_Animator.SetTrigger ("Dead");
			m_Animator.enabled = false;
			m_Animator.transform.SetParent (null);
			GameManager.Instance.PlayerDied ();
			gameObject.SetActive (false);
		}
	}

	public void GiveAmmo (int a_Ammo)
	{
		m_Ammo += a_Ammo;
		GameManager.Instance.HUD.UpdateAmmoCount (m_Ammo);
	}
}
