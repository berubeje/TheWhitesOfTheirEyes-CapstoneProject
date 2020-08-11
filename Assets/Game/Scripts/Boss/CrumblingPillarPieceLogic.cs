using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingPillarPieceLogic : MonoBehaviour
{
    public float timeTillFade = 5.0f;
    public float fadeTime = 3.0f;

    private bool fade = false;
    private float currentTime = 0.0f;
    private JimController _player;
    private Material _mat;
    private MeshRenderer _meshRenderer;



    // Start is called before the first frame update
    void Start()
    {
        _player = InputManager.Instance.jimController;

        if(_player != null)
        {
            Physics.IgnoreCollision(_player.GetComponent<Collider>(), GetComponent<Collider>(), true);
        }

        _mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (fade == false)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= timeTillFade)
            {
                fade = true;
                currentTime = 0.0f;
            }
        }
        else
        {
            Color newColor = _mat.color;
            

            if (fadeTime > 0.0f)
            {
                newColor.a -= Time.deltaTime / fadeTime;
            }
            else
            {
                newColor.a -= Time.deltaTime;
            }

            _mat.color = newColor;


            if (newColor.a <= 0.0f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if (transform.parent != null)
        {
            if (transform.parent.childCount <= 1)
            {
                Destroy(transform.parent.gameObject);
            }
        }
    }
}
