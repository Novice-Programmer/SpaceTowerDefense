using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField] MeshRenderer _planetRenderer = null;
    [SerializeField] Material[] _normalMaterials = null;
    [SerializeField] Material[] _lockMaterials = null;

    [SerializeField] GameObject _lockObject = null;
    [SerializeField] GameObject _canvas = null;
    [SerializeField] Transform _planetCamera = null;
    [SerializeField] float _rotateSpeed = 30.0f;

    public bool _stageOpen;
    public EPlanetType _planetType;

    bool _selected = false;

    private void Update()
    {
        if (_selected)
        {
            _planetRenderer.transform.Rotate(Vector3.up * Time.deltaTime * _rotateSpeed);
        }
    }

    private void LateUpdate()
    {
        if (_planetCamera.gameObject.activeSelf)
            _canvas.transform.LookAt(_planetCamera);
    }

    public void StageCheck(bool stageOpen)
    {
        _stageOpen = stageOpen;
        _lockObject.SetActive(!_stageOpen);
        if (_stageOpen)
        {
            for (int i = 0; i < _planetRenderer.materials.Length; i++)
            {
                _planetRenderer.materials[i] = _normalMaterials[i];
            }
        }
        else
        {
            for (int i = 0; i < _planetRenderer.materials.Length; i++)
            {
                _planetRenderer.materials[i] = _lockMaterials[i];
            }
        }
    }

    public void Select()
    {
        _canvas.SetActive(false);
        _selected = true;
    }

    public void NoneSelect()
    {
        _canvas.SetActive(true);
        _selected = false;
    }
}
