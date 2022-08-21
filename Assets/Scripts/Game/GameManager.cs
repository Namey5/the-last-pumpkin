using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[SerializeField] private Camera m_MainCamera;
	[SerializeField] private HUD m_HUD;
	[SerializeField] private PlayerController m_Player;

	private int m_ZombieKills = 0;

	public Camera MainCamera => m_MainCamera;
	public HUD HUD => m_HUD;
	public PlayerController Player => m_Player;

	public int ZombieKills => m_ZombieKills;

	private void Awake ()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Debug.LogError ("[GameManager] THERE CAN BE ONLY ONE");
			Destroy (this);
			return;
		}
	}

	public void ZombieKilled ()
	{
		m_ZombieKills++;
		m_HUD.UpdateKillCount (m_ZombieKills);
	}
}
