using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[SerializeField] private Canvas m_Canvas;
	[SerializeField] private Image m_Fader;
	[SerializeField] private string m_GameSceneName = "game";

	private void Start ()
	{
		m_Fader.enabled = false;
	}

	public void Play ()
	{
		StartCoroutine (FadeAndPlay ());
	}

	private IEnumerator FadeAndPlay ()
	{
		const float FADE_DURATION = 1f;

		m_Fader.enabled = true;

		// Graphic.CrossFadeAlpha() doesn't seem to work, so doing this manually
		float t = 0f;
		while (t < 1f)
		{
			t += Time.unscaledDeltaTime / FADE_DURATION;
			m_Fader.color = Color.Lerp (Color.clear, Color.black, Mathf.Clamp01 (t));
			yield return null;
		}

		// Wait a lil' bit more :)
		yield return new WaitForSecondsRealtime (0.5f);

		yield return SceneManager.LoadSceneAsync (m_GameSceneName);
	}

	public void ShowSettings ()
	{

	}

	public void Quit ()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.ExitPlaymode ();
#else
		Application.Quit ();
#endif
	}
}
