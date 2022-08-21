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

	private Rigidbody[] m_RagdollBodies;

	private int m_Health = 100;
	private bool m_IsAlive = true;

	private void Awake ()
	{
		m_RagdollBodies = GetComponentsInChildren<Rigidbody> ();
		SetRagdoll (false);
	}

	private void Update ()
	{
		if (Input.GetKeyDown (KeyCode.K))
		{
			m_NavMeshAgent.destination = transform.position + (new Vector3 (Random.value, 0f, Random.value) * 2f - Vector3.one) * 4f;
		}

		float _CurrentSpeed = Mathf.Clamp01 (m_NavMeshAgent.velocity.magnitude / m_NavMeshAgent.speed);
		m_Animator.SetFloat ("Speed", _CurrentSpeed);
	}

	private void SetRagdoll (bool a_RagdollEnabled)
	{
		m_Animator.enabled = !a_RagdollEnabled;
		for (int i = 0; i < m_RagdollBodies.Length; i++)
		{
			m_RagdollBodies[i].isKinematic = !a_RagdollEnabled;
		}
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
		m_Animator.transform.SetParent (null, true);
		SetRagdoll (true);
		m_RagdollRoot.AddForce (a_Force * 100f);
		Destroy (gameObject);
	}
}
