using UnityEngine;
using UnityEngine.UI;

public class CinematicBars : Singleton<CinematicBars>
{
    private RectTransform _topBar;
    private RectTransform _bottomBar;

    private float _scrollSpeed;
    private float _barSize;
    private float _initialSize;
    private float _endSize;
    private float _interpolant;
    private bool _isActive;

    private void Awake()
    {
        GameObject gameObject = new GameObject("TopBar", typeof(Image));
        gameObject.transform.SetParent(transform, false);
        gameObject.GetComponent<Image>().color = Color.black;
        _topBar = gameObject.GetComponent<RectTransform>();
        _topBar.anchorMin = new Vector2(-0.1f, 1);
        _topBar.anchorMax = new Vector2(1.1f, 1);
        _topBar.sizeDelta = new Vector2(0, 0);

        gameObject = new GameObject("BottomBar", typeof(Image));
        gameObject.transform.SetParent(transform, false);
        gameObject.GetComponent<Image>().color = Color.black;
        _bottomBar = gameObject.GetComponent<RectTransform>();
        _bottomBar.anchorMin = new Vector2(-0.1f, 0);
        _bottomBar.anchorMax = new Vector2(1.1f, 0);
        _bottomBar.sizeDelta = new Vector2(0, 0);
    }

    private void Update()
    {
        if (_isActive)
        {
            Vector2 sizeDelta = Vector2.zero;
            _interpolant += _scrollSpeed * Time.deltaTime;

            sizeDelta.y = Mathf.Lerp(_initialSize, _endSize, _interpolant);

            _topBar.sizeDelta = sizeDelta;
            _bottomBar.sizeDelta = sizeDelta;

            if(_interpolant >= 1)
            {
                _interpolant = 0;
                _isActive = false;
            }
        }
    }

    public void ShowBars(float size, float speed)
    {
        if (!_isActive)
        {
            _barSize = size;
            _initialSize = 0;
            _endSize = _barSize;
            _scrollSpeed = speed;
            _isActive = true;
        }
    }

    public void HideBars(float speed)
    {
        if (!_isActive)
        {
            _initialSize = _barSize;
            _endSize = 0;
            _barSize = 0;
            _scrollSpeed = speed;
            _isActive = true;
        }
    }
}
