using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public enum EndType
{
	Died,
	Lost,
	Won
}

[System.Serializable]
public struct EndData
{
	public EndType type;
	public string title;
	public string description;
	public Sprite image;
}

public class EndMenu : MonoBehaviour
{
	[SerializeField] private TMP_Text m_Title;
	[SerializeField] private TMP_Text m_Description;
	[SerializeField] private TMP_Text m_KillCount;
	[SerializeField] private Image m_Image;

	[SerializeField] private EndData[] m_Scenarios = new EndData[0];

	public static EndType EndType = EndType.Died;
	public static int ZombiesKilled = 0;

	private void Start ()
	{
		for (int i = 0; i < m_Scenarios.Length; i++)
		{
			if (m_Scenarios[i].type == EndType)
			{
				m_Title.text = m_Scenarios[i].title;
				m_Description.text = m_Scenarios[i].description;
				m_KillCount.text = $"Zombies Killed: {ZombiesKilled}";
				m_Image.sprite = m_Scenarios[i].image;

				return;
			}
		}
	}

	public void Retry ()
	{
		SceneManager.LoadScene ("game");
	}

	public void Exit ()
	{
		SceneManager.LoadScene ("menu");
	}
}
