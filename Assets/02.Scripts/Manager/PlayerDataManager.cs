using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : TSingleton<PlayerDataManager>
{
    Dictionary<EResearchType, Dictionary<int, EResearch>> _playerSelectResearch;
    Dictionary<EObjectType, List<EObjectName>> _playerSelectObject;
    Dictionary<EObjectType, List<EObjectName>> _playerAvailableObject;
    Dictionary<EResearchType, int> _playerResearchFacility;
    Dictionary<EPlanetType, bool> _playerAvailablePlanet;
    Dictionary<EPlanetType, List<bool>> _playerAvailableStage;

    int _clearStage = 0;
    int _maxSelectObject = 2;
    int _addSelectObject = 0;

    public int MaxSelected { get { return _maxSelectObject + _addSelectObject; } }
    public int ClearStage { get { return _clearStage; } }
    public int SelectedNumber
    {
        get
        {
            int selectNumber = 0;
            foreach (List<EObjectName> objectNames in _playerSelectObject.Values)
            {
                selectNumber += objectNames.Count;
            }
            return selectNumber;
        }
    }

    public int SelectedTowerNumber
    {
        get
        {
            int selectedTowerNum = 0;
            foreach (EObjectType objectType in _playerSelectObject.Keys)
            {
                if (objectType == EObjectType.Tower)
                    selectedTowerNum = _playerSelectObject[EObjectType.Tower].Count;
            }
            return selectedTowerNum;
        }
    }

    public bool Selected { get { return SelectedNumber < _maxSelectObject + _addSelectObject; } }

    List<EResearch> SelectResearchList
    {
        get
        {
            List<EResearch> researches = new List<EResearch>();
            foreach (Dictionary<int, EResearch> stepResearch in _playerSelectResearch.Values)
            {
                foreach (EResearch research in stepResearch.Values)
                {
                    researches.Add(research);
                }
            }
            return researches;
        }
    }

    public List<ObjectData> SelectList
    {
        get
        {
            List<ObjectData> selectObjects = new List<ObjectData>();
            foreach (EObjectType objectType in _playerSelectObject.Keys)
            {
                List<EObjectName> objectNames = _playerSelectObject[objectType];
                for (int i = 0; i < objectNames.Count; i++)
                {
                    selectObjects.Add(new ObjectData(objectType, objectNames[i]));
                }
            }
            return selectObjects;
        }
    }

    public List<ObjectData> AvailableList
    {
        get
        {
            List<ObjectData> availableObjects = new List<ObjectData>();
            foreach (EObjectType objectType in _playerAvailableObject.Keys)
            {
                List<EObjectName> objectNames = _playerAvailableObject[objectType];
                for (int i = 0; i < objectNames.Count; i++)
                {
                    availableObjects.Add(new ObjectData(objectType, objectNames[i]));
                }
            }
            return availableObjects;
        }
    }

    public Dictionary<EPlanetType, bool> AvailablePlanet { get { return _playerAvailablePlanet; } }

    private void Awake()
    {
        Init();
        Instance = this;
    }

    public void LoadData(Dictionary<EResearchType, Dictionary<int, EResearch>> selectResearch, Dictionary<EObjectType, List<EObjectName>> selectObject,
        Dictionary<EResearchType, int> checkResearch, Dictionary<EObjectType, List<EObjectName>> availableObject,
            Dictionary<EPlanetType, bool> availablePlanet, Dictionary<EPlanetType, List<bool>> availableStage)
    {
        if (selectResearch != null)
        {
            _playerSelectResearch = selectResearch;
            ResearchManager.Instance.ResearchUpdate(SelectResearchList);
        }
        else
        {
            _playerSelectResearch = new Dictionary<EResearchType, Dictionary<int, EResearch>>();
        }
        if (selectObject != null)
        {
            _playerSelectObject = selectObject;
        }
        else
        {
            _playerSelectObject = new Dictionary<EObjectType, List<EObjectName>>();
            List<EObjectName> towerNames = new List<EObjectName>();
            towerNames.Add(EObjectName.KW9A);
            _playerSelectObject.Add(EObjectType.Tower, towerNames);
            List<EObjectName> obstacleNames = new List<EObjectName>();
            obstacleNames.Add(EObjectName.FireWall);
            _playerSelectObject.Add(EObjectType.Obstacle, obstacleNames);
        }

        if (checkResearch != null)
        {
            _playerResearchFacility = checkResearch;
        }
        else
        {
            _playerResearchFacility = new Dictionary<EResearchType, int>();
            _playerResearchFacility.Add(EResearchType.Facility, -1);
            _playerResearchFacility.Add(EResearchType.Tower, -1);
            _playerResearchFacility.Add(EResearchType.Obstacle, -1);
            _playerResearchFacility.Add(EResearchType.Resource, -1);
            ResearchManager.Instance.ResearchUpdate(SelectResearchList);
        }
        if (availableObject != null)
        {
            _playerAvailableObject = availableObject;
        }
        else
        {
            _playerAvailableObject = new Dictionary<EObjectType, List<EObjectName>>();
            _playerAvailableObject.Add(EObjectType.Tower, new List<EObjectName>());
            _playerAvailableObject[EObjectType.Tower].Add(EObjectName.KW9A);
            _playerAvailableObject.Add(EObjectType.Obstacle, new List<EObjectName>());
            _playerAvailableObject[EObjectType.Obstacle].Add(EObjectName.FireWall);
        }
        if (availablePlanet != null)
        {
            _playerAvailablePlanet = availablePlanet;
        }
        else
        {
            _playerAvailablePlanet = new Dictionary<EPlanetType, bool>();
            _playerAvailablePlanet.Add(EPlanetType.Mars, true);
        }
        if (availableStage != null)
        {
            _playerAvailableStage = new Dictionary<EPlanetType, List<bool>>();
            List<bool> _marsList = new List<bool>();
            _marsList.Add(true);
            _playerAvailableStage.Add(EPlanetType.Mars, _marsList);
        }
    }

    public void ResearchUpdate(EResearchType researchType, int step, EResearch research)
    {
        Dictionary<int, EResearch> stepResearch;
        if (_playerSelectResearch.ContainsKey(researchType))
        {
            stepResearch = _playerSelectResearch[researchType];
        }
        else
        {
            stepResearch = new Dictionary<int, EResearch>();
            _playerSelectResearch.Add(researchType, stepResearch);
        }
        if (stepResearch.ContainsKey(step))
        {
            if (research == EResearch.None)
            {
                stepResearch.Remove(step);
            }
            else
            {
                stepResearch[step] = research;
            }
        }
        else
        {
            stepResearch.Add(step, research);
        }
        ResearchManager.Instance.ResearchUpdate(SelectResearchList);
    }

    public EResearch GetSelectResearch(EResearchType researchType, int step)
    {
        if (_playerSelectResearch.ContainsKey(researchType))
        {
            Dictionary<int, EResearch> stepResearch = _playerSelectResearch[researchType];
            if (stepResearch.ContainsKey(step))
                return _playerSelectResearch[researchType][step];
        }
        return EResearch.None;
    }

    public bool GetResearchCheck(EResearchType researchType, int step)
    {
        if (_playerResearchFacility[researchType] >= step)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SelectObjectUpdate(EObjectType objectType, EObjectName objectName)
    {
        List<EObjectName> objectNames;
        if (_playerSelectObject.ContainsKey(objectType))
        {
            objectNames = _playerSelectObject[objectType];
        }
        else
        {
            objectNames = new List<EObjectName>();
            _playerSelectObject.Add(objectType, objectNames);
        }
        objectNames.Add(objectName);
    }

    public void SelectedCancleUpdate(EObjectType objectType, EObjectName objectName)
    {
        List<EObjectName> objectNames = _playerSelectObject[objectType];
        for (int i = 0; i < objectNames.Count; i++)
        {
            if (objectName == objectNames[i])
            {
                objectNames.RemoveAt(i);
                break;
            }
        }
    }

    public void AvailableObjectUpdate(EObjectType objectType, EObjectName objectName)
    {
        List<EObjectName> objectNames;
        if (_playerAvailableObject.ContainsKey(objectType))
        {
            objectNames = _playerAvailableObject[objectType];
        }
        else
        {
            objectNames = new List<EObjectName>();
            _playerAvailableObject.Add(objectType, objectNames);
        }
        objectNames.Add(objectName);
    }

    public void StageClear(int stage)
    {
        if (_clearStage < stage)
        {
            _clearStage = stage;
        }
    }

    public void FacilityUpdate(EResearchType researchType, int level)
    {
        if (researchType == EResearchType.Facility)
        {
            _addSelectObject = level;
        }
        else if (researchType == EResearchType.Tower)
        {
            if (level == 1)
            {
                _playerAvailableObject[EObjectType.Tower].Add(EObjectName.P013);
            }
        }
        else if (researchType == EResearchType.Obstacle)
        {
            if (level == 1)
            {
                _playerAvailableObject[EObjectType.Obstacle].Add(EObjectName.Swamp);
            }
        }
        _playerResearchFacility[researchType] = level;
    }

    public int GetFacilityLevel(EResearchType researchType)
    {
        return _playerResearchFacility[researchType];
    }

    public Dictionary<EResearchType, int> GetFacilityData()
    {
        return _playerResearchFacility;
    }
}
