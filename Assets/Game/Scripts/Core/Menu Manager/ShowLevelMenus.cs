using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLevelMenus : MonoBehaviour
{
	public MenuClassifier menuToShow;
	public MenuClassifier menuToHide;

    void Start()
    {
		MenuManager.Instance.showMenu(menuToShow);
		MenuManager.Instance.hideMenu(menuToHide);
    }
}
