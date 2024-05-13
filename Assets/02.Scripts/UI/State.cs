using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EStateType
{
    Idle,
    Delay,
    Stun,
    Breakdown,
    AttackSearch,
    Attack,
    AttackCommander,
    Move,
    Die,

    None
}

public class State : MonoBehaviour
{
    [SerializeField] Sprite[] _stateSprites = null;
    [SerializeField] SpriteRenderer _stateRenderer = null;
    [SerializeField] float _viewTime = 5.0f;


    float _timeCheck = 0f;
    private void Update()
    {
        _timeCheck += Time.deltaTime;
        if (_timeCheck >= _viewTime)
        {
            _timeCheck = 0;
            gameObject.SetActive(false);
        }
    }

    public void StateUpdate(EStateType stateType)
    {
        gameObject.SetActive(true);
        _timeCheck = 0;
        _stateRenderer.sprite = _stateSprites[(int)stateType];
        switch (stateType)
        {
            case EStateType.Idle:
            case EStateType.Delay:
                _stateRenderer.color = Color.white;
                break;
            case EStateType.Stun:
                _stateRenderer.color = Color.yellow;
                break;
            case EStateType.Breakdown:
                _stateRenderer.color = Color.black;
                break;
            case EStateType.AttackSearch:
                _stateRenderer.color = Color.green;
                break;
            case EStateType.Attack:
            case EStateType.AttackCommander:
                _stateRenderer.color = Color.red;
                break;
            case EStateType.Move:
                _stateRenderer.color = Color.blue;
                break;
            case EStateType.Die:
                _stateRenderer.color = Color.black;
                break;
        }
    }
}
