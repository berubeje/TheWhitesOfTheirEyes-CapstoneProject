using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
	public MenuClassifier menuToShow;
	public MenuClassifier menuToHide;

	public void onButtonClicked()
	{
		MenuManager.Instance.showMenu(menuToShow);
		MenuManager.Instance.hideMenu(menuToHide);
	}
}
