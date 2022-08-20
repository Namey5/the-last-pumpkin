using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
	[Header ("Components")]
	[SerializeField] private CharacterController m_CharacterController;
	[SerializeField] private Animator m_Animator;

	private Rigidbody[] m_RagdollBodies;

	private void Awake ()
	{
		m_RagdollBodies = GetComponentsInChildren<Rigidbody> ();
		SetRagdoll (false);
	}

	private void SetRagdoll (bool a_RagdollEnabled)
	{
		m_Animator.enabled = !a_RagdollEnabled;
		for (int i = 0; i < m_RagdollBodies.Length; i++)
		{
			m_RagdollBodies[i].isKinematic = !a_RagdollEnabled;
		}
	}

	private void Update ()
	{
		if (Input.GetKeyDown (KeyCode.J))
		{
			m_CharacterController.enabled = false;
			SetRagdoll (true);
		}
	}
}
