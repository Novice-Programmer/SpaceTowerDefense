using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyHologram : MonoBehaviour
{
    GameObject[] _hologramObject;
    Transform _nowObjectTransform;
    int _viewObjectNumber = 0;
    float _rotateSpeed = 30;
    float _nextObjectTime = 7.0f;
    float _timeCheck = 0.0f;

    private void Start()
    {
        _hologramObject = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            _hologramObject[i] = transform.GetChild(i).gameObject;
            _hologramObject[i].SetActive(false);
        }
        RotateObjectSetting();
    }

    private void Update()
    {
        _timeCheck += Time.deltaTime;
        if (_timeCheck >= _nextObjectTime)
        {
            RotateObjectSetting();
        }
        _nowObjectTransform.Rotate(Vector3.up * Time.deltaTime * _rotateSpeed);
    }

    void RotateObjectSetting()
    {
        _timeCheck = 0;
        if (_nowObjectTransform != null)
            _nowObjectTransform.gameObject.SetActive(false);
        _hologramObject[_viewObjectNumber].SetActive(true);
        _nowObjectTransform = _hologramObject[_viewObjectNumber++].transform;
        if (_viewObjectNumber >= _hologramObject.Length)
        {
            _viewObjectNumber = 0;
        }
    }
}
