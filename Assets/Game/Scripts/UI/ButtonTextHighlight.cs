using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTextHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,  ISelectHandler, IDeselectHandler
{
    public TMP_Text text;
    public TMP_FontAsset defaultFont;
    public TMP_FontAsset selectedFont;


    public void OnPointerEnter(PointerEventData eventData)
    {
        text.font = selectedFont;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.font = defaultFont;
    }

    public void OnSelect(BaseEventData eventData)
    {
        text.font = selectedFont;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        text.font = defaultFont;
    }

    private void OnDisable()
    {
        text.font = defaultFont;
    }
}
