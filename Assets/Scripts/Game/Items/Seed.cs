using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
	[SerializeField] private float m_TimeToGrow = 15f;
	[SerializeField] private float m_MinScale = 0.5f;
	[SerializeField] private float m_MaxScale = 2f;
	[SerializeField] private int m_MinDrops = 1;
	[SerializeField] private int m_MaxDrops = 3;
	[SerializeField] private SeedResult[] m_ItemPickups = new SeedResult[0];

	private float m_Time = 0f;

	private void Update ()
	{
		m_Time += Time.deltaTime;

		float t = Mathf.Clamp01 (m_Time / m_TimeToGrow);
		transform.localScale = Vector3.one * Mathf.Lerp (m_MinScale, m_MaxScale, t);

		if (m_Time >= m_TimeToGrow)
		{
			SpawnPickups ();
			Destroy (gameObject);
		}
	}

	private void SpawnPickups ()
	{
		int _Count = Random.Range (m_MinDrops, m_MaxDrops);
		for (int i = 0; i < _Count; i++)
		{
			Vector3 _Position = transform.position;
			_Position.y += 0.3f;
			_Position.x += Mathf.Sin (i / (float)_Count * Mathf.PI * 2f) * (_Count - 1f) * 0.25f;
			_Position.z += Mathf.Cos (i / (float)_Count * Mathf.PI * 2f) * (_Count - 1f) * 0.25f;

			SpawnRandomPickup (_Position);
		}
	}

	private void SpawnRandomPickup (Vector3 a_Position)
	{
		float _TotalChance = 0f;
		for (int i = 0; i < m_ItemPickups.Length; i++)
		{
			_TotalChance += m_ItemPickups[i].dropChance;
		}

		float _Rand = Random.value * _TotalChance;
		for (int i = 0; i < m_ItemPickups.Length; i++)
		{
			_Rand -= m_ItemPickups[i].dropChance;
			if (_Rand <= 0f)
			{
				Instantiate (m_ItemPickups[i].item, a_Position, Quaternion.identity);
				return;
			}
		}
	}
}

[System.Serializable]
public struct SeedResult
{
	public float dropChance;
	public ItemPickup item;
}
