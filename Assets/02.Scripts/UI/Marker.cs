using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    [SerializeField] SpriteRenderer _markIcon = null;
    [SerializeField] SpriteRenderer _markBackground = null;
    [SerializeField] float _markHeight = 10.0f;
    Transform _target;

    public void MarkerSetting(Transform target, Sprite iconSprite, Sprite backgroundSprite)
    {
        _target = target;
        _markIcon.sprite = iconSprite;
        if (backgroundSprite != null)
            _markBackground.sprite = backgroundSprite;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null || (_target != null && !_target.gameObject.activeSelf))
        {
            Destroy(gameObject);
            return;
        }
        Vector3 pos = _target.transform.position;
        pos.y += _markHeight;
        transform.position = pos;
    }
}
