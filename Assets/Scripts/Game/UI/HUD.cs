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
	[SerializeField] private HUDInventorySlot[] m_InventorySlots;

	private int m_SelectedSlot;

	private void Awake ()
	{
		UpdateKillCount (0);
		UpdateAmmoCount (0);
		UpdatePlayerHealth (100);
		UpdateFarmDefences (100);

		for (int i = 0; i < m_InventorySlots.Length; i++)
		{
			m_InventorySlots[i].SetSlotID (i);
		}
		SelectInventorySlot (0);
	}

	public void UpdateKillCount (int a_Kills)
	{
		m_KillCountText.text = a_Kills.ToString ();
	}

	public void UpdateAmmoCount (int a_Ammo)
	{
		m_AmmoText.text = a_Ammo.ToString ();
	}

	public void UpdatePlayerHealth (int a_Health)
	{
		m_HealthBar.rectTransform.anchorMax = new Vector2 (1f, Mathf.Clamp01 (a_Health / 100f));
	}

	public void UpdateFarmDefences (int a_Defences)
	{
		m_DefencesBar.rectTransform.anchorMax = new Vector2 (Mathf.Clamp01 (a_Defences / 100f), 1f);
	}

	public void SelectInventorySlot (int a_Slot)
	{
		if (m_InventorySlots[m_SelectedSlot] != null)
		{
			m_InventorySlots[m_SelectedSlot].SetInactive ();
		}

		m_SelectedSlot = a_Slot;
		m_InventorySlots[m_SelectedSlot].SetActive ();
	}
}
