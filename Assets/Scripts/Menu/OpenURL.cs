using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenURL : MonoBehaviour
{
	[SerializeField] private string m_URL;

	public void Open ()
	{
		if (!string.IsNullOrEmpty (m_URL))
		{
			Application.OpenURL (m_URL);
		}
	}
}
