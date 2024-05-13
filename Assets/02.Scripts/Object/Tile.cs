using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ETileType
{
    FieldTile,
    TowerTile,
    ObstacleTile,

    None
}

public enum EFitType
{
    PXPYFits,
    MXPYFits,
    PXMYFits,
    MXMYFits,
    EXPYFits,
    EXMYFits,
    PXEYFits,
    MXEYFits,
    EXEYFits,
    Overlaps,
    OutOfBounds
}

public class Tile : MonoBehaviour
{
    [SerializeField] ETileType _tileType = ETileType.FieldTile;
    [SerializeField] Node _nodePrefab = null;
    [SerializeField] IntVector2 _dimensions = IntVector2.zero;
    [SerializeField] float gridSize = 1;

    bool[,] _availableNodes;
    Node[,] _nodeTiles;

    private void Awake()
    {
        _nodeTiles = new Node[_dimensions.x, _dimensions.y];
        _availableNodes = new bool[_dimensions.x, _dimensions.y];
        for (int y = 0; y < _dimensions.y; y++)
        {
            for (int x = 0; x < _dimensions.x; x++)
            {
                Vector3 targetPos = new Vector3(x + 0.5f, 0.01f, y + 0.5f) * gridSize;
                Node nodeTile = Instantiate(_nodePrefab, transform);
                nodeTile._parentTile = this;
                nodeTile.transform.localPosition = targetPos;
                _nodeTiles[x, y] = nodeTile;
                nodeTile._pos = new IntVector2(x, y);
                nodeTile.StateSet(_tileType);
            }
        }
    }

    public IntVector2 WorldToGrid(Vector3 worldLocation, IntVector2 sizeOffset)
    {
        Vector3 localLocation = transform.InverseTransformPoint(worldLocation);

        var offset = new Vector3(sizeOffset.x * 0.5f, 0.0f, sizeOffset.y * 0.5f);
        localLocation -= offset;

        int xPos = Mathf.RoundToInt(localLocation.x);
        int yPos = Mathf.RoundToInt(localLocation.z);

        return new IntVector2(xPos, yPos);
    }

    public Vector3 NodeToPosition(IntVector2 nodePos, IntVector2 size, EFitType fitType)
    {
        Vector3 standardNodePos = _nodeTiles[nodePos.x, nodePos.y].transform.position;
        Vector3 lastNodePos;
        IntVector2 sizeValue = new IntVector2(size.x - 1, size.y - 1);
        IntVector2 lastPos;
        switch (fitType)
        {
            case EFitType.MXPYFits: // 왼쪽 위로 배치 가능
                lastPos = new IntVector2(nodePos.x - sizeValue.x, nodePos.y + sizeValue.y);
                break;
            case EFitType.PXMYFits: // 오른쪽 아래로 배치 가능
                lastPos = new IntVector2(nodePos.x + sizeValue.x, nodePos.y - sizeValue.y);
                break;
            case EFitType.MXMYFits: // 왼쪽 아래로 배치 가능
                lastPos = new IntVector2(nodePos.x - sizeValue.x, nodePos.y - sizeValue.y);
                break;
            case EFitType.EXPYFits: // 오른쪽 끝으로 위로 배치 가능
                lastPos = new IntVector2(_dimensions.x - 1, nodePos.y + sizeValue.y);
                standardNodePos = _nodeTiles[_dimensions.x - size.x, nodePos.y].transform.position;
                break;
            case EFitType.EXMYFits:// 오른쪽 끝으로 아래로 배치 가능
                lastPos = new IntVector2(_dimensions.x - 1, nodePos.y - sizeValue.y);
                standardNodePos = _nodeTiles[_dimensions.x - size.x, nodePos.y].transform.position;
                break;
            case EFitType.PXEYFits: // 오른쪽 위 끝으로 배치 가능
                lastPos = new IntVector2(nodePos.x + sizeValue.x, _dimensions.y - 1);
                standardNodePos = _nodeTiles[nodePos.x, _dimensions.y - size.y].transform.position;
                break;
            case EFitType.MXEYFits: // 왼쪽 위 끝으로 배치 가능
                lastPos = new IntVector2(nodePos.x - sizeValue.x, _dimensions.y - 1);
                standardNodePos = _nodeTiles[nodePos.x, _dimensions.y - size.y].transform.position;
                break;
            case EFitType.EXEYFits: // 오른쪽 끝 위 끝으로 배치 가능
                lastPos = new IntVector2(_dimensions.x - 1, _dimensions.y - 1);
                standardNodePos = _nodeTiles[_dimensions.x - size.x, _dimensions.y - size.y].transform.position;
                break;
            default: // 오른쪽 위로 배치 가능
                lastPos = sizeValue + nodePos;
                break;
        }
        lastNodePos = _nodeTiles[lastPos.x, lastPos.y].transform.position;
        return (lastNodePos + standardNodePos) / 2f;
    }

    public EFitType Fits(IntVector2 gridPos, IntVector2 size)
    {
        if ((size.x > _dimensions.x) || (size.y > _dimensions.y))
        {
            return EFitType.OutOfBounds;
        }

        if (gridPos.x < 0 || gridPos.y < 0)
        {
            return EFitType.OutOfBounds;
        }

        EFitType fitType = EFitType.OutOfBounds;

        if (gridPos.x + size.x <= _dimensions.x && gridPos.y + size.y <= _dimensions.y)
        {
            bool overlaps = false;
            IntVector2 extents = gridPos + size;
            for (int y = gridPos.y; y < extents.y; y++)
            {
                for (int x = gridPos.x; x < extents.x; x++)
                {
                    if (_availableNodes[x, y])
                    {
                        fitType = EFitType.Overlaps;
                        overlaps = true;
                        break;
                    }
                }
            }
            if (!overlaps)
                fitType = EFitType.PXPYFits;
        }

        else if (gridPos.x + size.x <= _dimensions.x && gridPos.y - (size.y - 1) >= 0)
        {
            bool overlaps = false;
            IntVector2 extents = new IntVector2(gridPos.x + size.x, gridPos.y - (size.y - 1));
            for (int y = gridPos.y; y >= extents.y; y--)
            {
                for (int x = gridPos.x; x < extents.x; x++)
                {
                    if (_availableNodes[x, y])
                    {
                        fitType = EFitType.Overlaps;
                        overlaps = true;
                        break;
                    }
                }
            }
            if (!overlaps)
                fitType = EFitType.PXMYFits;
        }

        else if (gridPos.x - (size.x - 1) >= 0 && gridPos.y + size.y <= _dimensions.y)
        {
            bool overlaps = false;
            IntVector2 extents = new IntVector2(gridPos.x - (size.x - 1), gridPos.y + size.y);
            for (int y = gridPos.y; y < extents.y; y++)
            {
                for (int x = gridPos.x; x >= extents.x; x--)
                {
                    if (_availableNodes[x, y])
                    {
                        fitType = EFitType.Overlaps;
                        overlaps = true;
                        break;
                    }
                }
            }
            if (!overlaps)
                fitType = EFitType.MXPYFits;
        }

        else if (gridPos.x - (size.x - 1) >= 0 && gridPos.y - (size.y - 1) >= 0)
        {
            bool overlaps = false;
            IntVector2 extents = gridPos - new IntVector2(size.x - 1, size.y - 1);
            for (int y = gridPos.y; y >= extents.y; y--)
            {
                for (int x = gridPos.x; x >= extents.x; x--)
                {
                    if (_availableNodes[x, y])
                    {
                        fitType = EFitType.Overlaps;
                        overlaps = true;
                        break;
                    }
                }
            }
            if (!overlaps)
                fitType = EFitType.MXMYFits;
        }

        else if (gridPos.x - (size.x - 1) < 0 && gridPos.y + size.y <= _dimensions.y)
        {
            bool overlaps = false;
            IntVector2 extents = new IntVector2(_dimensions.x - size.x, gridPos.y + size.y);
            for (int y = gridPos.y; y < extents.y; y++)
            {
                for (int x = _dimensions.x - 1; x >= extents.x; x--)
                {
                    if (_availableNodes[x, y])
                    {
                        fitType = EFitType.Overlaps;
                        overlaps = true;
                        break;
                    }
                }
            }
            if (!overlaps)
                fitType = EFitType.EXPYFits;
        }
        else if (gridPos.x - (size.x - 1) < 0 && gridPos.y - (size.y - 1) >= 0)
        {
            bool overlaps = false;
            IntVector2 extents = new IntVector2(_dimensions.x - size.x, gridPos.y - (size.y - 1));
            for (int y = gridPos.y; y >= extents.y; y--)
            {
                for (int x = _dimensions.x - 1; x >= extents.x; x--)
                {
                    if (_availableNodes[x, y])
                    {
                        fitType = EFitType.Overlaps;
                        overlaps = true;
                        break;
                    }
                }
            }
            if (!overlaps)
                fitType = EFitType.EXMYFits;
        }
        else if (gridPos.x + size.x <= _dimensions.x && gridPos.y - (size.y - 1) < 0)
        {
            bool overlaps = false;
            IntVector2 extents = new IntVector2(gridPos.x + size.x, _dimensions.y - size.y);
            for (int y = _dimensions.y - 1; y >= extents.y; y--)
            {
                for (int x = gridPos.x; x < extents.x; x++)
                {
                    if (_availableNodes[x, y])
                    {
                        fitType = EFitType.Overlaps;
                        overlaps = true;
                        break;
                    }
                }
            }
            if (!overlaps)
                fitType = EFitType.PXEYFits;
        }
        else if (gridPos.y - (size.y - 1) >= 0 && gridPos.y - (size.y - 1) < 0)
        {
            bool overlaps = false;
            IntVector2 extents = new IntVector2(gridPos.x - (size.y - 1), _dimensions.y - size.y);
            for (int y = _dimensions.y - 1; y >= extents.y; y--)
            {
                for (int x = gridPos.x; x >= extents.x; x--)
                {
                    if (_availableNodes[x, y])
                    {
                        fitType = EFitType.Overlaps;
                        overlaps = true;
                        break;
                    }
                }
            }
            if (!overlaps)
                fitType = EFitType.MXEYFits;
        }
        else if (gridPos.x - (size.x - 1) < 0 && gridPos.y - (size.y - 1) < 0)
        {
            bool overlaps = false;
            IntVector2 extents = new IntVector2(_dimensions.x - size.x, _dimensions.y - size.y);
            for (int y = _dimensions.y - 1; y >= extents.y; y--)
            {
                for (int x = _dimensions.x - 1; x >= extents.x; x--)
                {
                    if (_availableNodes[x, y])
                    {
                        fitType = EFitType.Overlaps;
                        overlaps = true;
                        break;
                    }
                }
            }
            if (!overlaps)
                fitType = EFitType.EXEYFits;
        }
        return fitType;
    }

    public void Occupy(IntVector2 gridPos, IntVector2 size, EFitType fitType)
    {
        IntVector2 extents;
        switch (fitType)
        {
            case EFitType.MXPYFits:
                extents = new IntVector2(_dimensions.x - 1, gridPos.y + size.y);
                for (int y = gridPos.y; y < extents.y; y++)
                {
                    for (int x = extents.x; x >= _dimensions.x - size.x; x--)
                    {
                        _availableNodes[x, y] = true;
                        _nodeTiles[x, y].StateSet(_tileType, ENodeState.Filled);
                    }
                }
                break;
            case EFitType.PXMYFits:
                extents = new IntVector2(gridPos.x + size.x, _dimensions.y - 1);
                for (int y = extents.y; y >= _dimensions.y - size.y; y--)
                {
                    for (int x = gridPos.x; x < extents.x; x++)
                    {
                        _availableNodes[x, y] = true;
                        _nodeTiles[x, y].StateSet(_tileType, ENodeState.Filled);
                    }
                }
                break;
            case EFitType.MXMYFits:
                extents = _dimensions + new IntVector2(-1, -1);
                for (int y = extents.y; y >= _dimensions.y - size.y; y--)
                {
                    for (int x = extents.x; x >= _dimensions.x - size.x; x--)
                    {
                        _availableNodes[x, y] = true;
                        _nodeTiles[x, y].StateSet(_tileType, ENodeState.Filled);
                    }
                }
                break;
            case EFitType.EXPYFits:
                extents = new IntVector2(_dimensions.x - 1, gridPos.y + size.y);
                for (int y = gridPos.y; y >= extents.y - 1; y--)
                {
                    for (int x = extents.x; x >= _dimensions.x - size.x; x--)
                    {
                        _availableNodes[x, y] = true;
                        _nodeTiles[x, y].StateSet(_tileType, ENodeState.Filled);
                    }
                }
                break;
            case EFitType.EXMYFits:
                extents = new IntVector2(_dimensions.x - 1, gridPos.y - size.y);
                for (int y = gridPos.y; y >= extents.y - 1; y--)
                {
                    for (int x = extents.x; x >= _dimensions.x - size.x; x--)
                    {
                        _availableNodes[x, y] = true;
                        _nodeTiles[x, y].StateSet(_tileType, ENodeState.Filled);
                    }
                }
                break;
            case EFitType.PXEYFits:
                extents = new IntVector2(gridPos.x + size.x, _dimensions.y - 1);
                for (int y = extents.y; y >= _dimensions.y - size.y; y--)
                {
                    for (int x = gridPos.x; x < extents.x; x++)
                    {
                        _availableNodes[x, y] = true;
                        _nodeTiles[x, y].StateSet(_tileType, ENodeState.Filled);
                    }
                }
                break;
            case EFitType.MXEYFits:
                extents = new IntVector2(gridPos.x - size.x, _dimensions.y - 1);
                for (int y = extents.y; y >= _dimensions.y - size.y; y--)
                {
                    for (int x = gridPos.x; x >= extents.x - 1; x--)
                    {
                        _availableNodes[x, y] = true;
                        _nodeTiles[x, y].StateSet(_tileType, ENodeState.Filled);
                    }
                }
                break;
            case EFitType.EXEYFits:
                extents = new IntVector2(_dimensions.x - 1, _dimensions.y - 1);
                for (int y = extents.y; y >= _dimensions.y - size.y; y--)
                {
                    for (int x = extents.x; x >= _dimensions.x - size.x; x--)
                    {
                        _availableNodes[x, y] = true;
                        _nodeTiles[x, y].StateSet(_tileType, ENodeState.Filled);
                    }
                }
                break;
            default:
                extents = gridPos + size;
                for (int y = gridPos.y; y < extents.y; y++)
                {
                    for (int x = gridPos.x; x < extents.x; x++)
                    {
                        _availableNodes[x, y] = true;
                        _nodeTiles[x, y].StateSet(_tileType, ENodeState.Filled);
                    }
                }
                break;
        }
    }

    public void Clear(IntVector2 gridPos, IntVector2 size, EFitType fitType)
    {
        IntVector2 extents;
        switch (fitType)
        {
            case EFitType.MXPYFits:
                extents = new IntVector2(_dimensions.x - 1, gridPos.y + size.y);
                for (int y = gridPos.y; y < extents.y; y++)
                {
                    for (int x = extents.x; x >= _dimensions.x - size.x; x--)
                    {
                        _availableNodes[x, y] = false;
                        _nodeTiles[x, y].StateSet(_tileType);
                    }
                }
                break;
            case EFitType.PXMYFits:
                extents = new IntVector2(gridPos.x + size.x, _dimensions.y - 1);
                for (int y = extents.y; y >= _dimensions.y - size.y; y--)
                {
                    for (int x = gridPos.x; x < extents.x; x++)
                    {
                        _availableNodes[x, y] = false;
                        _nodeTiles[x, y].StateSet(_tileType);
                    }
                }
                break;
            case EFitType.MXMYFits:
                extents = _dimensions + new IntVector2(-1, -1);
                for (int y = extents.y; y >= _dimensions.y - size.y; y--)
                {
                    for (int x = extents.x; x >= _dimensions.x - size.x; x--)
                    {
                        _availableNodes[x, y] = false;
                        _nodeTiles[x, y].StateSet(_tileType);
                    }
                }
                break;
            case EFitType.EXPYFits:
                extents = new IntVector2(_dimensions.x - 1, gridPos.y + size.y);
                for (int y = gridPos.y; y >= extents.y - 1; y--)
                {
                    for (int x = extents.x; x >= _dimensions.x - size.x; x--)
                    {
                        _availableNodes[x, y] = false;
                        _nodeTiles[x, y].StateSet(_tileType);
                    }
                }
                break;
            case EFitType.EXMYFits:
                extents = new IntVector2(_dimensions.x - 1, gridPos.y - size.y);
                for (int y = gridPos.y; y >= extents.y - 1; y--)
                {
                    for (int x = extents.x; x >= _dimensions.x - size.x; x--)
                    {
                        _availableNodes[x, y] = false;
                        _nodeTiles[x, y].StateSet(_tileType);
                    }
                }
                break;
            case EFitType.PXEYFits:
                extents = new IntVector2(gridPos.x + size.x, _dimensions.y - 1);
                for (int y = extents.y; y >= _dimensions.y - size.y; y--)
                {
                    for (int x = gridPos.x; x < extents.x; x++)
                    {
                        _availableNodes[x, y] = false;
                        _nodeTiles[x, y].StateSet(_tileType);
                    }
                }
                break;
            case EFitType.MXEYFits:
                extents = new IntVector2(gridPos.x - size.x, _dimensions.y - 1);
                for (int y = extents.y; y >= _dimensions.y - size.y; y--)
                {
                    for (int x = gridPos.x; x >= extents.x - 1; x--)
                    {
                        _availableNodes[x, y] = false;
                        _nodeTiles[x, y].StateSet(_tileType);
                    }
                }
                break;
            case EFitType.EXEYFits:
                extents = new IntVector2(_dimensions.x - 1, _dimensions.y - 1);
                for (int y = extents.y; y >= _dimensions.y - size.y; y--)
                {
                    for (int x = extents.x; x >= _dimensions.x - size.x; x--)
                    {
                        _availableNodes[x, y] = false;
                        _nodeTiles[x, y].StateSet(_tileType);
                    }
                }
                break;
            default:
                extents = gridPos + size;
                for (int y = gridPos.y; y < extents.y; y++)
                {
                    for (int x = gridPos.x; x < extents.x; x++)
                    {
                        _availableNodes[x, y] = false;
                        _nodeTiles[x, y].StateSet(_tileType);
                    }
                }
                break;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // Validate grid size
        if (gridSize <= 0)
        {
            gridSize = 1;
        }

        // Validate dimensions
        if (_dimensions.x <= 0)
        {
            _dimensions.x = 1;
        }
        if (_dimensions.y <= 0)
        {
            _dimensions.y = 1;
        }
    }

    /// <summary>
    /// Draw the grid in the scene view
    /// </summary>
    void OnDrawGizmos()
    {
        Color prevCol = Gizmos.color;
        if (_tileType == ETileType.FieldTile)
        {
            Gizmos.color = Color.green;
        }
        else if (_tileType == ETileType.TowerTile)
        {
            Gizmos.color = Color.cyan;
        }
        else if (_tileType == ETileType.ObstacleTile)
        {
            Gizmos.color = Color.yellow;
        }

        else
        {
            Gizmos.color = Color.black;
        }

        Matrix4x4 originalMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        // Draw local space flattened cubes
        for (int y = 0; y < _dimensions.y; y++)
        {
            for (int x = 0; x < _dimensions.x; x++)
            {
                var position = new Vector3((x + 0.5f) * gridSize, 0, (y + 0.5f) * gridSize);
                Gizmos.DrawWireCube(position, new Vector3(gridSize, 0, gridSize));
            }
        }

        Gizmos.matrix = originalMatrix;
        Gizmos.color = prevCol;
    }
#endif
}
