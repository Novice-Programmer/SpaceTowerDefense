using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESoundBGM
{
    Lobby,
    Mars,
    MissionFail,
    MissionComplete
}

public enum ESoundName
{
    UIOpen,
    UIClose,
    ButtonClick,
    ResearchChange,
    Installation,
    LoadEnd,
    PlayerMove,
    DoorOpen1,
    DoorOpen2,
    DoorOpen3,
    Rocket,
    RocketBomb,
    Nuclear,
    NuclearBomb,
    FireBallBomb,
    GetMineral,
    InstallObject,
    NMDADie,
    Repair,
    SellObject,
    TowerHit,
    Upgrade,
    FireBall,
    ClearStage,
    FailStage,
    InfoView,
    SelectObject,
    FireBress,
    Wing,
    Laser,
    ChargingLaser,
    BDUDie,
    FacilityUpdate,
    WaveStart,
    WaveEnd,
    BossAper,
    MoneyAdd
}
public class SoundManager : TSingleton<SoundManager>
{
    float _volumeBgm = 0.5f;
    float _volumeEff = 0.5f;

    bool _muteBgm = false;
    bool _muteEff = false;

    [SerializeField] AudioClip[] _bgmClips = null;
    [SerializeField] AudioClip[] _soundClips = null;
    [SerializeField] GameObject _prefabSoundSource = null;
    [SerializeField] int _audioSourceInitNumber = 30;
    [SerializeField] int _audioMaxOverlap = 5;

    AudioSource _playerBGM;

    List<SoundSource> _ltSound = new List<SoundSource>();
    Dictionary<ESoundName, int> _soundActiveNumber = new Dictionary<ESoundName, int>();

    public float VolumeBGM
    {
        get { return _volumeBgm; }
        private set
        {
            _volumeBgm = value;
            _playerBGM.volume = value;
        }
    }

    public float VolumeEffect
    {
        get { return _volumeEff; }
        private set { _volumeEff = value; }
    }

    public bool MuteBGM
    {
        get { return _muteBgm; }
        private set
        {
            _muteBgm = value;
            _playerBGM.mute = value;
        }
    }

    public bool MuteEff
    {
        get { return _muteEff; }
        private set { _muteEff = value; }
    }

    private void Awake()
    {
        Instance = this;
        Init();
        for (int i = 0; i < _audioSourceInitNumber; i++)
        {
            SoundSource soundSource = InitSoundSource();
            _ltSound.Add(soundSource);
            soundSource.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < _ltSound.Count; i++)
        {
            if (i >= _audioSourceInitNumber) // 제한 갯수를 넘어서 생성된거 처리
            {
                if (_ltSound[i].gameObject.activeSelf)
                {
                    if (!_ltSound[i]._audioSource.isPlaying)
                    {
                        _soundActiveNumber[_ltSound[i]._soundName]--;
                        Destroy(_ltSound[i].gameObject);
                        _ltSound.RemoveAt(i);
                        break;
                    }
                }
            }
            else
            {
                if (_ltSound[i].gameObject.activeSelf)
                {
                    if (!_ltSound[i]._audioSource.isPlaying)
                    {
                        _soundActiveNumber[_ltSound[i]._soundName]--;
                        _ltSound[i].gameObject.SetActive(false);
                        break;
                    }
                }
            }
        }
    }

    SoundSource InitSoundSource()
    {
        GameObject go = Instantiate(_prefabSoundSource, transform);
        SoundSource soundSource = go.GetComponent<SoundSource>();
        soundSource._audioSource.volume = _volumeEff;
        soundSource._audioSource.mute = _muteEff;
        return soundSource;
    }

    public void PlayBGMSound(ESoundBGM bgmType)
    {
        if (_playerBGM != null)
            Destroy(_playerBGM.gameObject);
        GameObject obj = new GameObject("BGMPlayer");
        obj.transform.parent = Camera.main.transform;
        obj.transform.localPosition = Vector3.zero;

        _playerBGM = obj.AddComponent<AudioSource>();
        _playerBGM.clip = _bgmClips[(int)bgmType];
        _playerBGM.volume = _volumeBgm;
        _playerBGM.mute = _muteBgm;
        _playerBGM.loop = true;
        _playerBGM.Play();
    }

    public SoundSource PlayEffectSound(ESoundName soundType, Transform owner, float soundDistance = 50.0f)
    {
        SoundSource soundSource = null;
        if (_soundActiveNumber.ContainsKey(soundType))
        {
            if (_soundActiveNumber[soundType] < _audioMaxOverlap)
            {
                _soundActiveNumber[soundType]++;
                for (int i = 0; i < _ltSound.Count; i++)
                {
                    if (!_ltSound[i].gameObject.activeSelf)
                    {
                        soundSource = _ltSound[i];
                        soundSource.gameObject.SetActive(true);
                        soundSource._soundName = soundType;
                        soundSource._audioSource.clip = _soundClips[(int)soundType];
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _ltSound.Count; i++)
                {
                    if (_ltSound[i].gameObject.activeSelf)
                    {
                        if (_ltSound[i]._soundName == soundType)
                        {
                            soundSource = _ltSound[i];
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            _soundActiveNumber.Add(soundType, 1);
            for (int i = 0; i < _ltSound.Count; i++)
            {
                if (!_ltSound[i].gameObject.activeSelf)
                {
                    soundSource = _ltSound[i];
                    soundSource.gameObject.SetActive(true);
                    soundSource._soundName = soundType;
                    soundSource._audioSource.clip = _soundClips[(int)soundType];
                    break;
                }
            }
        }
        if (soundSource == null)
        {
            soundSource = InitSoundSource();
        }
        soundSource._audioSource.minDistance = soundDistance * 0.1f;
        soundSource._audioSource.maxDistance = soundDistance;
        if (owner != null)
            soundSource.transform.position = owner.position;
        else
            soundSource.transform.position = Camera.main.transform.position;
        soundSource._audioSource.Play();
        return soundSource;
    }
}
