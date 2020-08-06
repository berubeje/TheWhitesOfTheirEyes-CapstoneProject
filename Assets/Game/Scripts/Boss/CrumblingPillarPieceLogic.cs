using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingPillarPieceLogic : MonoBehaviour
{
    public float timeTillDestroy = 3.0f;

    private float currentTimeToDestroy = 0.0f;
    private JimController _player;


    // Start is called before the first frame update
    void Start()
    {
        _player = InputManager.Instance.jimController;

        if(_player != null)
        {
            Physics.IgnoreCollision(_player.GetComponent<Collider>(), GetComponent<Collider>(), true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTimeToDestroy += Time.deltaTime;

        if(currentTimeToDestroy >= timeTillDestroy)
        {
            Destroy(this.gameObject);
        }
    }
}
