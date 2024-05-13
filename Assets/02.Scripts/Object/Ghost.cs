using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ERotateType
{
    degree0,
    degree90,
    degree180,
    degree270
}

public class Ghost : MonoBehaviour
{
    public EObjectType _objectType = EObjectType.None;
    public EObjectName _objectName = EObjectName.None;
    public int _installCost;
    public IntVector2 _demision;
    IntVector2 _saveDemision;
    public IntVector2 _gridPos;
    public EFitType _fitType = EFitType.Overlaps;
    public Vector3 _fitPos;
    public Tile _parentTile;
    public ERotateType _rotateType = ERotateType.degree0;
    [SerializeField] MeshRenderer[] _materialRenders = null;
    [SerializeField] Material _fitMaterial = null;
    [SerializeField] Material _outMaterial = null;
    [SerializeField] GameObject _rangeObject = null;

    private void Start()
    {
        _saveDemision = _demision;
        if (_objectType == EObjectType.Tower)
        {
            float totalRange;
            ResearchResult researchResult = ResearchManager.Instance.GameResearchData;
            float range = ObjectDataManager.Instance.GetTowerData(_objectName).atkRange;
            if (researchResult.startATKUpgrade != 0)
            {
                float upgradeRange = ObjectDataManager.Instance.GetUpgradeData(_objectName, EUpgradeType.Attack, researchResult.startATKUpgrade).addValue[2];
                totalRange = range + upgradeRange + ((range + upgradeRange) * researchResult.atkRangeAddRate * 0.01f);
            }
            else
            {
                totalRange = range + (range * researchResult.atkRangeAddRate * 0.01f);
            }
            _rangeObject.transform.localScale *= totalRange;
        }
    }

    public void FitMaterialCheck(EFitType towerFitType)
    {
        switch (towerFitType)
        {
            case EFitType.PXPYFits:
            case EFitType.MXPYFits:
            case EFitType.PXMYFits:
            case EFitType.MXMYFits:
            case EFitType.EXPYFits:
            case EFitType.EXMYFits:
            case EFitType.PXEYFits:
            case EFitType.MXEYFits:
            case EFitType.EXEYFits:
                MaterialApply(_fitMaterial);
                break;
            case EFitType.Overlaps:
                MaterialApply(_outMaterial);
                break;
            case EFitType.OutOfBounds:
                MaterialApply(_outMaterial);
                break;
        }
    }

    public void NoneCheck()
    {
        MaterialApply(_outMaterial);
    }

    void MaterialApply(Material material)
    {
        for (int i = 0; i < _materialRenders.Length; i++)
        {
            _materialRenders[i].material = material;
        }
    }

    public void RotateObject(bool rotate = false)
    {
        if (rotate)
        {
            if (_rotateType == ERotateType.degree270)
            {
                _rotateType = ERotateType.degree0;
            }

            else
            {
                _rotateType++;
            }
        }
        else
        {
            if (_rotateType != ERotateType.degree0)
                _rotateType = ERotateType.degree0;
            else
                return;
        }

        Vector3 rotateEuler = Vector3.zero;

        switch (_rotateType)
        {
            case ERotateType.degree0:
                _demision = _saveDemision;
                break;
            case ERotateType.degree90:
                rotateEuler = new Vector3(0, 90, 0);
                _demision = new IntVector2(_saveDemision.y, _saveDemision.x);
                break;
            case ERotateType.degree180:
                rotateEuler = new Vector3(0, 180, 0);
                _demision = _saveDemision;
                break;
            case ERotateType.degree270:
                rotateEuler = new Vector3(0, 270, 0);
                _demision = new IntVector2(_saveDemision.y, _saveDemision.x);
                break;
        }

        transform.rotation = Quaternion.Euler(rotateEuler);

        if (_parentTile != null)
        {
            Vector3 rotateAddEuler = _parentTile.transform.rotation.eulerAngles + rotateEuler;
            transform.rotation = Quaternion.Euler(rotateAddEuler);
        }
    }
}
