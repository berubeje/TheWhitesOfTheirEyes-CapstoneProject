using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTitleScreenController : MonoBehaviour
{
    public Light leftEyeLight;
    public Light rightEyeLight;
        
    public float lightIntensityRange;
    public float lightNoiseRange;
    public float positionRange;
    public float angleRange;

    public float lightLerpSpeed;
    public float transformLerpSpeed;

    private float _minLightInstensity;
    private float _maxLightInstensity;

    private Vector3 _minPosition;
    private Vector3 _maxPosition;

    private void Awake()
    {
        _minLightInstensity = leftEyeLight.intensity;
        _maxLightInstensity = leftEyeLight.intensity + lightIntensityRange;

        _minPosition = transform.position;
        _minPosition.y -= positionRange;
        _minPosition.z -= positionRange;

        _maxPosition = transform.position;
        _maxPosition.y += positionRange;
        _maxPosition.z += positionRange;


    }

    // Update is called once per frame
    void Update()
    {
        LerpLightIntensity();
        LerpTransform();
    }

    private void LerpLightIntensity()
    {
        float newIntensity = Mathf.Lerp(_minLightInstensity, _maxLightInstensity, Mathf.PingPong(Time.time * lightLerpSpeed, 1) );

        leftEyeLight.intensity = newIntensity + Random.Range(-lightNoiseRange, lightNoiseRange);
        rightEyeLight.intensity = newIntensity + Random.Range(-lightNoiseRange, lightNoiseRange);
    }

    private void LerpTransform()
    {
        Vector3 newPosition = Vector3.Lerp(_minPosition, _maxPosition, Mathf.PingPong(Time.time * transformLerpSpeed, 1));
        float newAngle = Mathf.Lerp(-angleRange, angleRange, Mathf.PingPong(Time.time * transformLerpSpeed, 1));

        transform.position = newPosition;
        transform.eulerAngles = new Vector3(newAngle, 180, 0);
    }
}
