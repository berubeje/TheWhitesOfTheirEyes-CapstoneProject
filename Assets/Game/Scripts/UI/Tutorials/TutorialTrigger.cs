using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    public Image tutorialPanel;
    public Text tutorialText;
    public float fadeSpeed;
    public float displayTime; 

    private Collider _collider;

    [HideInInspector]
    public bool _triggered;

    private bool _displayed;
    private float _interpolant;
    private float _elapsedDisplayTime;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (_triggered)
        {
            FadeIn();
        }

        if (_displayed)
        {
            if(_elapsedDisplayTime >= displayTime)
            {
                FadeOut();
            }

            _elapsedDisplayTime += Time.deltaTime;
        }
                
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(_collider);
            _triggered = true;
        }
    }

    private void FadeIn()
    {
        float panelAlpha = Mathf.Lerp(0, 200, _interpolant);
        float textAlpha = Mathf.Lerp(0, 255, _interpolant);

        tutorialPanel.color = new Vector4(0, 0, 0, panelAlpha/255);
        tutorialText.color = new Vector4(255, 255, 255, textAlpha / 255);

        if(_interpolant >= 1)
        {
            _interpolant = 1;
            _triggered = false;
            _displayed = true;
        }

        _interpolant += Time.deltaTime * fadeSpeed;
    }

    private void FadeOut()
    {
        float panelAlpha = Mathf.Lerp(0, 200, _interpolant);
        float textAlpha = Mathf.Lerp(0, 255, _interpolant);

        tutorialPanel.color = new Vector4(0, 0, 0, panelAlpha / 255);
        tutorialText.color = new Vector4(255, 255, 255, textAlpha / 255);

        if (_interpolant <= 0)
        {
            _interpolant = 0;
            Destroy(this);
        }

        _interpolant -= Time.deltaTime * fadeSpeed;
    }
}
