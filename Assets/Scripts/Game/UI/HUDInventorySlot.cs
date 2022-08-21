using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDInventorySlot : MonoBehaviour
{
	[Header ("Components")]
	[SerializeField] private Image m_SlotBackground;
	[SerializeField] private Image m_SlotIcon;
	[SerializeField] private TMP_Text m_SlotNumberText;

	[Header ("Misc")]
	[SerializeField] private Sprite m_NormalBackground;
	[SerializeField] private Sprite m_SelectedBackground;

	private int m_SlotID;

	public void SetSlotID (int a_SlotID)
	{
		m_SlotID = a_SlotID;
		m_SlotNumberText.text = (m_SlotID + 1).ToString ();
	}

	public void SetActive ()
	{
		m_SlotBackground.sprite = m_SelectedBackground;
	}

	public void SetInactive ()
	{
		m_SlotBackground.sprite = m_NormalBackground;
	}
}
