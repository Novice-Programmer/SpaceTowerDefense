using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commander : ObjectGame
{
    public static Commander Instance { set; get; }
    WorldStatusUI _statusUI;

    [SerializeField] int _hp = 500;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _statusUI = ObjectDataManager.Instance.StatusInit();
        _statusUI.StatusSetting(transform, _hp, 9999, false);
        GameUI.Instance.CommanderSetting(_hp);
        ObjectDataManager.Instance.MarkerSetting(transform, EObjectName.Commander);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Hit(int damage, EWeakType weakType)
    {
        if (!GameManager.Instance.GameEndCheck)
        {
            _hp -= damage;
            _statusUI.HPChange(_hp);
            if (_hp <= 0)
            {
                _hp = 0;
                GameManager.Instance.GameEnd(false);
            }
            GameUI.Instance.CommanderHit(_hp);
        }
    }
}
