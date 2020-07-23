using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
	public MenuClassifier menuToShow;
	public MenuClassifier menuToHide;

    private void Awake()
    {
		MenuManager.Instance.showMenu(menuToShow);
		MenuManager.Instance.hideMenu(menuToHide);
	}
}
