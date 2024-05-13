using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Transform _target = null;
    [SerializeField] Vector3 _offSet = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        transform.position = _target.position + _offSet;
    }
}
