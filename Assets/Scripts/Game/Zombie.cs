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

	private void Awake ()
	{
		m_RagdollBodies = GetComponentsInChildren<Rigidbody> ();
		SetRagdoll (false);
	}

	private void Update ()
	{
		if (Input.GetKeyDown (KeyCode.J))
		{
			Kill (Vector3.forward * 5000f);
		}

		if (Input.GetKeyDown (KeyCode.K))
		{
			m_NavMeshAgent.destination = transform.position + (new Vector3 (Random.value, 0f, Random.value) * 2f - Vector3.one) * 4f;
		}
	}

	private void SetRagdoll (bool a_RagdollEnabled)
	{
		m_Animator.enabled = !a_RagdollEnabled;
		for (int i = 0; i < m_RagdollBodies.Length; i++)
		{
			m_RagdollBodies[i].isKinematic = !a_RagdollEnabled;
		}
	}

	public void Kill (Vector3 a_Force = default)
	{
		m_Animator.transform.SetParent (null, true);
		SetRagdoll (true);
		m_RagdollRoot.AddForce (a_Force);
		Destroy (gameObject);
	}
}
