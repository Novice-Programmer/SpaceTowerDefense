using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENodeType
{
    TowerNode,
    ObstacleNode,
    FieldNode,

    None
}

public enum ENodeState
{
    Filled,
    Empty
}

public class Node : MonoBehaviour
{
	public float _height = 0.0f;
    public ENodeType _nodeType = ENodeType.None;
    public Tile _parentTile;
    public IntVector2 _pos;
    [SerializeField] Material _useTileM = null;
    [SerializeField] Material _towerTileM = null;
    [SerializeField] Material _obstacleTileM = null;

    public void StateSet(ETileType tileType, ENodeState nodeState = ENodeState.Empty)
    {
        if(tileType == ETileType.FieldTile || _nodeType == ENodeType.FieldNode)
        {
            _nodeType = ENodeType.FieldNode;
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            if (nodeState == ENodeState.Filled)
            {
                transform.GetChild(0).GetComponent<MeshRenderer>().material = _useTileM;
            }
            else
            {
                switch (tileType)
                {
                    case ETileType.TowerTile:
                        _nodeType = ENodeType.TowerNode;
                        transform.GetChild(0).GetComponent<MeshRenderer>().material = _towerTileM;
                        break;
                    case ETileType.ObstacleTile:
                        _nodeType = ENodeType.ObstacleNode;
                        transform.GetChild(0).GetComponent<MeshRenderer>().material = _obstacleTileM;
                        break;
                }
            }
        }
    }
}
