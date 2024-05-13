using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField] float _dotTime = 0.3f;
    [SerializeField] Image _imgLoading = null;
    [SerializeField] Text _txtLoading = null;
    [SerializeField] Text _txtLoadingValue = null;
    [SerializeField] GameObject[] _rotateObjects = null;
    [SerializeField] GameObject[] _lobbyViewObjects = null;
    [SerializeField] GameObject[] _gameViewObjects = null;
    [SerializeField] Text _txtName = null;
    [SerializeField] Text _txtExplan = null;

    float _rate = 0;
    float _timeCheck = 0;

    int _dotCount = 0;
    int _maxDotCount = 6;

    bool _inLobby = false;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _rotateObjects.Length; i++)
        {
            _rotateObjects[i].transform.Rotate(Vector3.up * Time.deltaTime * 30.0f);
        }
        if (_rate < 1)
        {
            if (_timeCheck > _dotTime)
            {
                _timeCheck = 0;
                _dotCount++;
                if (_dotCount > _maxDotCount)
                    _dotCount = 0;
                _txtLoading.text = "Loading.";
                for (int i = 0; i < _dotCount; i++)
                {
                    _txtLoading.text += ".";
                }
            }
        }
    }

    public void OpenLoaddingWnd(ESceneType type)
    {
        _rate = 0;
        _txtLoadingValue.text = "0%";
        if (type == ESceneType.Lobby)
        {
            _inLobby = true;
        }
        else
        {
            _inLobby = false;
        }

        if (_inLobby)
        {
            for (int i = 0; i < _lobbyViewObjects.Length; i++)
            {
                _lobbyViewObjects[i].SetActive(true);
            }
            _txtName.text = "함선";
            _txtExplan.text = "타워와 방해물 관련  \n" +
                               "연구가 가능하며," +
                               "다른 행성으로 포탈을 열어서 \n" +
                               "이동이 가능하다. \n" +
                               "인류는 아직도 수 많은 \n" +
                               "정착지를 찾는 중이다...";

        }
        else
        {
            for (int i = 0; i < _gameViewObjects.Length; i++)
            {
                _gameViewObjects[i].SetActive(true);
            }
            _txtName.text = "화성";
            _txtExplan.text = "화성은 인류 정착지로  \n" +
                               "개발된 첫번째 구역이다." +
                               "화성의 자원도 서서히 \n" +
                               "떨어져 많은 사람들이 \n" +
                               "다른곳으로 이동하였다. \n" +
                               "현재는 화성에서 생긴 \n" +
                               "돌연변이 생명체들이 \n" +
                               "화성 인류를 위협한다.";
        }
    }

    public void SettingLoadRate(float rate)
    {
        _rate = rate;
        _imgLoading.fillAmount = rate;
        _txtLoadingValue.text = rate * 100 + "%";

        if (_rate == 1)
        {
            _txtLoading.text = "Just a moment, please";
        }
    }
}