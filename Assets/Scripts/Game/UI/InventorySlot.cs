using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
	[Header ("Inventory")]
	[SerializeField] private int m_SlotID;

	[Header ("Components")]
	[SerializeField] private Image m_SlotBackground;
	[SerializeField] private Image m_SlotIcon;
	[SerializeField] private TMP_Text m_SlotNumberText;

	[Header ("Misc")]
	[SerializeField] private Sprite m_NormalBackground;
	[SerializeField] private Sprite m_SelectedBackground;

	private void Start ()
	{
		m_SlotNumberText.text = (m_SlotID + 1).ToString ();
	}
}
