using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
	[SerializeField] private float m_Time = 0.05f;

	private void OnEnable ()
	{
		StartCoroutine (WaitAndDisable ());
	}

	private IEnumerator WaitAndDisable ()
	{
		yield return new WaitForSeconds (m_Time);
		gameObject.SetActive (false);
	}
}
