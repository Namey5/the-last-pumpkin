using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
	[Header ("Components")]
	[SerializeField] private NavMeshAgent m_NavMeshAgent;
	[SerializeField] private Animator m_Animator;
	[SerializeField] private Rigidbody m_RagdollRoot;
	[SerializeField] private ItemPickup m_AmmoDrop;

	[Header ("AI")]
	[SerializeField] private float m_ActivityDelay = 1f;
	[SerializeField] private float m_PlayerTargetDistance = 3f;
	[SerializeField] private float m_PlayerAttackDistance = 1f;
	[SerializeField] private float m_MinSpeed = 1f;
	[SerializeField] private float m_MaxSpeed = 2f;
	[SerializeField] private int m_MinPlayerDamage = 1;
	[SerializeField] private int m_MaxPlayerDamage = 4;
	[SerializeField] private int m_MinFarmDamage = 1;
	[SerializeField] private int m_MaxFarmDamage = 4;

	private Rigidbody[] m_RagdollBodies;

	private float m_RandomOffset;
	private Vector3 m_TargetPosition;

	private int m_Health = 100;
	private bool m_IsAlive = true;

	private void Awake ()
	{
		m_RagdollBodies = GetComponentsInChildren<Rigidbody> ();
		SetRagdoll (false);
	}

	private void Start ()
	{
		m_RandomOffset = Random.value;

		m_TargetPosition = GameManager.Instance.ZombieTarget.GetRandomPoint ();
		m_NavMeshAgent.destination = m_TargetPosition;

		m_NavMeshAgent.speed = Mathf.Lerp (m_MinSpeed, m_MaxSpeed, m_RandomOffset);
	}

	private void Update ()
	{
		if (!m_IsAlive)
		{
			return;
		}

		float _CurrentSpeed = Mathf.Clamp01 (m_NavMeshAgent.velocity.magnitude / m_NavMeshAgent.speed);
		m_Animator.SetFloat ("Speed", _CurrentSpeed);

		if ((Time.time + m_RandomOffset * m_ActivityDelay) % m_ActivityDelay < Time.deltaTime)
		{
			m_NavMeshAgent.isStopped = false;

			PlayerController _Player = GameManager.Instance.Player;
			Vector3 _PlayerVec = (_Player.transform.position - transform.position);
			float _PlayerDist = _PlayerVec.magnitude;
			if (_PlayerDist < m_PlayerTargetDistance)
			{
				m_NavMeshAgent.destination = _Player.transform.position;

				if (_PlayerDist < m_PlayerAttackDistance)
				{
					_PlayerVec.y = 0f;
					transform.rotation = Quaternion.LookRotation (_PlayerVec);

					m_NavMeshAgent.isStopped = true;
					AttackPlayer ();
				}
				else
				{
					m_NavMeshAgent.stoppingDistance = 0f;
				}
			}
			else if (Vector3.Distance (transform.position, m_TargetPosition) < 1f)
			{
				AttackFarm ();
			}
			else
			{
				m_NavMeshAgent.destination = m_TargetPosition;
			}
		}
	}

	private void AttackPlayer ()
	{
		m_Animator.SetTrigger ("Attack");
		GameManager.Instance.Player.Damage (Random.Range (m_MinPlayerDamage, m_MaxPlayerDamage));
	}

	private void AttackFarm ()
	{
		m_Animator.SetTrigger ("Attack");
		GameManager.Instance.FarmAttacked (Random.Range (m_MinFarmDamage, m_MaxFarmDamage));
	}

	public bool Damage (Vector3 a_Direction, int a_Damage)
	{
		m_Health -= a_Damage;

		m_IsAlive = m_Health > 0;
		if (!m_IsAlive)
		{
			Kill (a_Direction * a_Damage);
		}

		return m_IsAlive;
	}

	public void Kill (Vector3 a_Force = default)
	{
		GameManager.Instance.ZombieKilled ();

		m_Animator.transform.SetParent (null, true);
		SetRagdoll (true);
		m_RagdollRoot.AddForce (a_Force * 100f);

		if (Random.value > 0.6f)
		{
			Instantiate (m_AmmoDrop, transform.position, Quaternion.identity);
		}

		Destroy (gameObject);
	}

	private void SetRagdoll (bool a_RagdollEnabled)
	{
		m_Animator.enabled = !a_RagdollEnabled;
		for (int i = 0; i < m_RagdollBodies.Length; i++)
		{
			m_RagdollBodies[i].isKinematic = !a_RagdollEnabled;
		}
	}
}
