using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class LinkUtility : MonoBehaviour
{
	[SerializeField] private TMP_Text m_Text;
	[SerializeField] private Link[] m_Links = new Link[0];

	private Camera m_Camera;

	private void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Mouse0))
		{
			int _LinkIndex = TMP_TextUtilities.FindIntersectingLink (m_Text, Input.mousePosition, null);
			if (_LinkIndex != -1)
			{
				string _LinkID = m_Text.textInfo.linkInfo[_LinkIndex].GetLinkID ();
				foreach (Link _Link in m_Links)
				{
					if (_LinkID == _Link.ID && _Link.OnClick != null)
					{
						_Link.OnClick.Invoke ();
					}
				}
			}
		}
	}

	[Serializable]
	private class Link
	{
		public string ID;
		public UnityEvent OnClick;
	}
}
