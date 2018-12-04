using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnTrigger : MonoBehaviour
{
	[Tooltip("The name of the scene we should load")]
	public string sceneToLoad;
	[Tooltip("Do we use a tag for trigger detection ?")]
	public bool useTag;
	public string tagName;
	// Use this for initialization
	void Start ()
	{
		if (sceneToLoad == null)
		{
			sceneToLoad = SceneManager.GetActiveScene().name;
			Debug.Log("No scene to load, using current scene");
		}
	}

	private void OnTriggerEnter (Collider other)
	{
		if(enabled)
		{
			if (useTag)
			{
				SceneMgr sceneManager = FindObjectOfType<SceneMgr>();
				if (sceneManager == null)
				{
					Debug.Log("No Scene Manager found in the scene ! Add one to any object in the scene");
				}
				else
				{
					sceneManager.LoadScene(sceneToLoad);
				}
			}
			else
			{
				SceneMgr sceneManager = FindObjectOfType<SceneMgr>();
				if (sceneManager == null)
				{
					Debug.Log("No Scene Manager found in the scene ! Add one to any object in the scene");
				}
				else
				{
					sceneManager.LoadScene(sceneToLoad);
				}
			}
		}
		
		
	}
}
