using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
	[Header ("Components")]
	[SerializeField] private Canvas m_Canvas;
	[SerializeField] private TMP_Text m_ObjectiveText;
	[SerializeField] private TMP_Text m_KillCountText;
	[SerializeField] private TMP_Text m_AmmoText;
	[SerializeField] private Image m_HealthBar;
	[SerializeField] private Image m_DefencesBar;
	[SerializeField] private InventorySlot[] m_InventorySlots;

	private void Start ()
	{
		m_KillCountText.text = "0";
		m_AmmoText.text = "0";
		m_HealthBar.rectTransform.anchorMax = Vector2.one;
		m_DefencesBar.rectTransform.anchorMax = Vector2.one;
	}

	public void UpdateKillCount (int a_Kills)
	{
		m_KillCountText.text = a_Kills.ToString ();
	}
}
