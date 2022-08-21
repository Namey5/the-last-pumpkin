using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[Header ("References")]
	[SerializeField] private Camera m_MainCamera;
	[SerializeField] private HUD m_HUD;
	[SerializeField] private Canvas m_PauseMenu;
	[SerializeField] private PlayerController m_Player;
	[SerializeField] private Zombie m_ZombiePrefab;
	[SerializeField] private TargetableArea m_ZombieSpawn;
	[SerializeField] private TargetableArea m_ZombieTarget;
	[SerializeField] private GameObject m_Pumpkin;

	[Header ("Game")]
	[SerializeField] private int m_ZombieKillsToWin = 50;
	[SerializeField] private float m_MinPumpkinSize = 1f;
	[SerializeField] private float m_MaxPumpkinSize = 50f;
	[SerializeField] private float m_ZombieSpawnRate = 5f;

	private int m_ZombieKills = 0;
	private int m_FarmDefences = 100;

	public Camera MainCamera => m_MainCamera;
	public HUD HUD => m_HUD;
	public PlayerController Player => m_Player;
	public TargetableArea ZombieSpawn => m_ZombieSpawn;
	public TargetableArea ZombieTarget => m_ZombieTarget;

	public int ZombieKills => m_ZombieKills;
	public int FarmDefences => m_FarmDefences;

	public bool Paused = false;

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

	private void Update ()
	{
		if (Paused)
		{
			return;
		}

		if (Input.GetButtonDown ("Pause"))
		{
			ShowPauseMenu ();
		}

		if ((Time.time % m_ZombieSpawnRate) < Time.deltaTime)
		{
			Instantiate (m_ZombiePrefab, m_ZombieSpawn.GetRandomPoint (), Quaternion.identity);
		}
	}

	public void ZombieKilled ()
	{
		m_ZombieKills++;
		m_HUD.UpdateKillCount (m_ZombieKills);

		m_Pumpkin.transform.localScale = Vector3.one * Mathf.Lerp (m_MinPumpkinSize, m_MaxPumpkinSize, Mathf.Clamp01 (m_ZombieKills / (float)m_ZombieKillsToWin));
		if (m_ZombieKills >= m_ZombieKillsToWin)
		{
			Won ();
		}
	}

	public void FarmAttacked (int a_Damage)
	{
		m_FarmDefences -= a_Damage;
		m_HUD.UpdateFarmDefences (Mathf.Max (0, m_FarmDefences));

		if (m_FarmDefences <= 0)
		{
			DefencesDestroyed ();
		}
	}

	public void ShowPauseMenu ()
	{
		m_PauseMenu.gameObject.SetActive (true);
		Paused = true;
		Time.timeScale = 0f;
	}

	public void HidePauseMenu ()
	{
		m_PauseMenu.gameObject.SetActive (false);
		Paused = false;
		Time.timeScale = 1f;
	}

	public void QuitToMenu ()
	{
		SceneManager.LoadScene ("menu");
	}

	public void PlayerDied ()
	{
		EndMenu.EndType = EndType.Died;
		EndMenu.ZombiesKilled = m_ZombieKills;

		StartCoroutine (PlayerDied_Coroutine ());
	}

	private IEnumerator PlayerDied_Coroutine ()
	{
		yield return new WaitForSecondsRealtime (2f);
		SceneManager.LoadScene ("end");
	}

	public void DefencesDestroyed ()
	{
		EndMenu.EndType = EndType.Lost;
		EndMenu.ZombiesKilled = m_ZombieKills;

		SceneManager.LoadScene ("end");
	}

	public void Won ()
	{
		EndMenu.EndType = EndType.Won;
		EndMenu.ZombiesKilled = m_ZombieKills;
		SceneManager.LoadScene ("end");
	}
}
