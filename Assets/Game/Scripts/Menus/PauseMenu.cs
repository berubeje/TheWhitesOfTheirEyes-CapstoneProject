using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : Menu
{
	public SceneReference sceneToLoad;
	public SceneReference sceneToUnload;

	public void onLoadScene()
	{
		SceneLoader.Instance.UnloadScene(sceneToUnload);
		SceneLoader.Instance.LoadScene(sceneToLoad, true);
		MenuManager.Instance.hideMenu(menuClassifier);
	}
}
