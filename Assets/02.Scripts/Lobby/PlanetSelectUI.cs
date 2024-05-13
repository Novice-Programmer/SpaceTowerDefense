using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EPlanetType
{
    Mars,
    Friratel,
    Leon
}

public class PlanetSelectUI : SelectUI
{
    [Header("Object")]
    [SerializeField] Planet[] _planets = null;
    [SerializeField] Camera _planetCamera = null;
    [SerializeField] Vector3 _cameraCorrection = Vector3.zero;

    [Header("UI")]
    [SerializeField] Text _explanTxt = null;
    [SerializeField] Text _planetNameTxt = null;
    [SerializeField] Text _planetDesTxt = null;
    [SerializeField] Button _selectCancleBtn = null;
    [SerializeField] Button _warpBtn = null;

    Vector3 _startPos;
    Planet _selectPlanet;

    private void Awake()
    {
        Dictionary<EPlanetType, bool> availablePlanet = PlayerDataManager.Instance.AvailablePlanet;
        foreach (EPlanetType planetType in availablePlanet.Keys)
        {
            for (int i = 0; i < _planets.Length; i++)
            {
                if (_planets[i]._planetType == planetType)
                {
                    _planets[i].StageCheck(availablePlanet[planetType]);
                }
            }
        }
        _startPos = _planetCamera.transform.position;
    }

    private void Update()
    {
        Ray ray = _planetCamera.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << LayerMask.NameToLayer("Planet");

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
        {
            Planet planet = hit.transform.GetComponent<Planet>();
            if (planet._stageOpen && Input.GetMouseButtonUp(0))
            {
                _planetCamera.transform.position = planet.transform.position + _cameraCorrection;
                planet.Select();
                _selectPlanet = planet;
                PlanetUISetting(planet._planetType);
                SoundManager.Instance.PlayEffectSound(ESoundName.ButtonClick, null);
            }
        }
    }

    public override void Open(LobbyPlayer lobbyPlayer)
    {
        base.Open(lobbyPlayer);
        _planetCamera.gameObject.SetActive(true);
        CleanSetting();
        LobbyManager.Instance.PlanetSelect(true);
    }

    public override void Close()
    {
        base.Close();
        if (_selectPlanet != null)
        {
            _selectPlanet.NoneSelect();
        }
        _planetCamera.gameObject.SetActive(false);
        LobbyManager.Instance.PlanetSelect(false);
    }

    void PlanetUISetting(EPlanetType planetType)
    {
        if (PlayerDataManager.Instance.SelectedTowerNumber > 0)
        {
            _explanTxt.color = Color.blue;
            _explanTxt.text = "워프 준비중..";
            _warpBtn.interactable = true;
        }
        else
        {
            _explanTxt.color = Color.red;
            _explanTxt.text = "타워를 한개 이상 선택해주세요.";
            _warpBtn.interactable = false;
        }
        if (planetType == EPlanetType.Mars)
        {
            _planetNameTxt.text = "화성";
            _planetDesTxt.text = "인류 최초의 거주 행성으로 \n개발된 행성이다.\n 다양한 생명체들이 생겨나면서 \n 그 중 인류에 적대적인 \n외계 생명체가 생겨났다.";
        }
        _selectCancleBtn.gameObject.SetActive(true);
    }

    void CleanSetting()
    {
        _planetCamera.transform.position = _startPos;
        _explanTxt.color = Color.white;
        _explanTxt.text = "행성을 선택하세요";
        _planetNameTxt.text = "";
        _planetDesTxt.text = "";
        _selectCancleBtn.gameObject.SetActive(false);
        _warpBtn.interactable = false;
    }

    public void ClickBackButton()
    {
        _selectPlanet.NoneSelect();
        _selectPlanet = null;
        CleanSetting();
        SoundManager.Instance.PlayEffectSound(ESoundName.ButtonClick, null);
    }

    public void ClickWarpButton()
    {
        SceneControlManager.Instance.SceneChange(ESceneType.Ingame);
    }
}
