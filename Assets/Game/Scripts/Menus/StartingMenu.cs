using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingMenu : Menu
{
	public SceneReference sceneToLoad;
	public SceneReference sceneToUnload;

	public GameObject healthBar;
	public void onLoadScene()
	{
		Cursor.lockState = CursorLockMode.Locked;
		SceneLoader.Instance.UnloadScene(sceneToUnload);
		SceneLoader.Instance.LoadScene(sceneToLoad, true);
		MenuManager.Instance.hideMenu(menuClassifier);
		healthBar.SetActive(true);
	}
}
