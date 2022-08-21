using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Collider))]
public class ItemPickup : MonoBehaviour
{
	[SerializeField] private ItemDefinition m_ItemDefinition;
	[SerializeField] private float m_BobSpeed = 1f;
	[SerializeField] private float m_RotateSpeed = 90f;

	private Vector3 m_StartPos;

	private void Start ()
	{
		m_StartPos = transform.position;
	}

	private void Update ()
	{
		transform.position = m_StartPos + Vector3.up * Mathf.Sin (Time.time * m_BobSpeed * Mathf.PI) * 0.2f;
		transform.Rotate (Vector3.up, m_RotateSpeed * Time.deltaTime);
	}

	private void OnTriggerEnter (Collider a_Other)
	{
		if (a_Other.TryGetComponent (out PlayerController _Player))
		{
			_Player.GiveItem (m_ItemDefinition);
			Destroy (gameObject);
		}
	}
}
