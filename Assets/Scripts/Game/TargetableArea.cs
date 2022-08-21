using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TargetableArea : MonoBehaviour
{
	[SerializeField] private Vector3 m_Extents = new Vector3 (5f, 0f, 1f);
	[SerializeField] private Color m_DebugColor = new Color (1f, 0f, 0f, 0.5f);

	public Vector3 GetRandomPoint ()
	{
		Vector3 _Offset = new Vector3
		(
			Random.Range (-m_Extents.x, m_Extents.x),
			Random.Range (-m_Extents.y, m_Extents.y),
			Random.Range (-m_Extents.z, m_Extents.z)
		);

		return transform.position + _Offset;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos ()
	{
		Color _Prev = Gizmos.color;
		Gizmos.color = m_DebugColor;
		Gizmos.DrawCube (transform.position, m_Extents * 2f);
		Gizmos.color = _Prev;
	}
#endif
}
