using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointContainer : MonoBehaviour
{
    public static Transform[] _wayPoints;

    private void Awake()
    {
        _wayPoints = new Transform[transform.childCount];

        for(int i = 0; i < _wayPoints.Length; i++)
        {
            _wayPoints[i] = transform.GetChild(i);
            _wayPoints[i].GetComponent<WayPoint>().WayPointSetting(i);
        }
    }
}
