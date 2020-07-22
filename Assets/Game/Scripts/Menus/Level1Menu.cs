using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Menu : Menu
{
	public SceneReference sceneToLoad;
	public SceneReference sceneToUnLoad;

	public void onLoadScene()
	{
		SceneLoader.Instance.UnloadScene(sceneToUnLoad);
		SceneLoader.Instance.LoadScene(sceneToLoad, true);
		MenuManager.Instance.hideMenu(menuClassifier);
	}
}
