using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepAttackPlaceholderLogic : MonoBehaviour
{
    public float startingAngle = 35f;
    public float attackSpeed = 2f;
    public float totalRotation = 70f;
    public float damageDelt = 15.0f;

    private bool _attackStarted = false;

    private bool _disabledRotationSet = false;
    private Quaternion _disabledRotation;

    private Quaternion _startRotation;
    private Quaternion _targetRotation;
    private float _t;

    // Update is called once per frame
    private void OnEnable()
    {
        _t = 0.0f;
        _attackStarted = false;

        if(!_disabledRotationSet)
        {
            _disabledRotation = transform.localRotation;
        }
    }

    void Update()
    {
        if (_attackStarted)
        {
            _t += Time.deltaTime / attackSpeed;

            transform.localRotation = Quaternion.Lerp(_startRotation, _targetRotation, _t);

            if (_t >= 1f)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void StartAttack(bool startRight)
    {
        if(startRight)
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(0, startingAngle, 0));
            _startRotation = transform.localRotation;

            Vector3 sweepValue = new Vector3(0, totalRotation, 0);
            _targetRotation = Quaternion.Euler(_startRotation.eulerAngles - sweepValue);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles - new Vector3(0, startingAngle, 0));
            _startRotation = transform.localRotation;

            Vector3 sweepValue = new Vector3(0, totalRotation, 0);
            _targetRotation = Quaternion.Euler(_startRotation.eulerAngles + sweepValue);
        }

        _attackStarted = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        JimController jimController = collision.gameObject.GetComponent<JimController>();

        if (jimController != null)
        {
            jimController.currentHealth -= damageDelt;
        }
    }

    private void OnDisable()
    {
        transform.localRotation = _disabledRotation;
    }
}
