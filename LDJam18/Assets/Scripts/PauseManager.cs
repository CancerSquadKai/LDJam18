using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
	public GameObject pausePanel;

	bool isPaused = false;
	// Use this for initialization
	
	public void Continue()
	{
		isPaused = false;
		Time.timeScale = 1f;
		pausePanel.SetActive(false);
	}

	public void Quit ()
	{
		Application.Quit();
	}

	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			pausePanel.SetActive(!isPaused);

			if(!isPaused)
			{
				Time.timeScale = 0.01f;
			}
			else
			{
				Time.timeScale = 1f;
			}

			isPaused = !isPaused;
		}
	}
}
