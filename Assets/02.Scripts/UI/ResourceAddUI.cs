using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceAddUI : MonoBehaviour
{
    Animator _ctrlAni;
    [SerializeField] float _removeTime = 1.5f;
    [SerializeField] GameObject _partIcon = null;
    [SerializeField] GameObject _mineralIcon = null;
    [SerializeField] Text _valueTxt = null;

    float _timeCheck = 0.0f;

    private void Awake()
    {
        _ctrlAni = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        _timeCheck += Time.deltaTime;
        if (_timeCheck >= _removeTime)
        {
            _ctrlAni.SetBool("Remove", true);
        }
    }

    public void AddSetting(bool towerParts, int value)
    {
        if (towerParts)
        {
            _partIcon.SetActive(true);
            _mineralIcon.SetActive(false);
        }
        else
        {
            _partIcon.SetActive(false);
            _mineralIcon.SetActive(true);
        }
        if (value > 0)
        {
            _valueTxt.color = Color.yellow;
        }
        else
        {
            _valueTxt.color = Color.red;
        }
        _valueTxt.text = value.ToString();
    }

    public void RemoveStart()
    {
        _ctrlAni.SetBool("Remove", true);
    }

    public void RemoveEnd()
    {
        Destroy(gameObject);
    }
}
