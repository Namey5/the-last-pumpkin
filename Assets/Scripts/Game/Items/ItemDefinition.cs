using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDefinition : ScriptableObject
{
	public int slot;
	public Sprite icon;
	public ItemPickup pickupPrefab;
}
