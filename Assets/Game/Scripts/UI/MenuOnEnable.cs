using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuOnEnable: MonoBehaviour
{
    public Button button;
    public TMP_Text text;
    public TMP_FontAsset selectedFont;

    private void OnEnable()
    {
        button.Select();
        text.font = selectedFont;
    }

}
