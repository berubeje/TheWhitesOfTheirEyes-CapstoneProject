using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarOnEnable : MonoBehaviour
{
    public BossController boss;
    public float healthAnimationSpeed;

    
    private bool _healthBarStarted;
    private float _interpolant;

    private void Update()
    {
        if (_healthBarStarted && boss != null)
        {
            UICanvas.Instance.ChangeBossHealthBar(Mathf.Lerp(0, boss.currentBossHealth/boss.maxHealth, _interpolant));

            if (_interpolant >= 1)
            {
                _healthBarStarted = false;
            }

            _interpolant += Time.deltaTime * healthAnimationSpeed;
        }
    }

    private void OnEnable()
    {
        boss = InputManager.Instance.jimController.boss;
        _interpolant = 0;
        UICanvas.Instance.ChangeBossHealthBar(0);
        _healthBarStarted = true;
    }

    private void OnDisable()
    {
        _healthBarStarted = false;
    }
}
