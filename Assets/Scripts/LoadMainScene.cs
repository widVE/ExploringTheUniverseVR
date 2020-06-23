using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMainScene : MonoBehaviour
{
	[SerializeField]
	int MainSceneBuildIndex = 1;

	void Start()
	{
		Application.backgroundLoadingPriority = ThreadPriority.Low;
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(MainSceneBuildIndex,
																UnityEngine.SceneManagement.LoadSceneMode.Single);
	}
}
