using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItem : MonoBehaviour
{
	[SerializeField] private bool m_CanAim = false;

	public abstract void Use (PlayerController a_Player);
}
