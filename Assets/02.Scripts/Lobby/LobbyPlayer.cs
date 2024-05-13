using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour
{
    CharacterController _controller;
    Animator _playerAnim;
    [SerializeField] Button _actionBtn = null;
    [SerializeField] Text _actionTxt = null;
    [SerializeField] LobbyStick _stickMovement = null;

    [SerializeField] float _moveSpeed = 3.0f;
    [SerializeField] float _rotateSpeed = 60.0f;

    Vector3 mv;
    Vector3 rv;

    SelectObject _selectObject;
    SelectObject _onObject;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _playerAnim = GetComponent<Animator>();
        _actionBtn.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneControlManager.Instance.NowLoaddingState == ELoaddingState.None)
        {
            if (_selectObject == null)
            {
                float mx = _stickMovement.DirMove.x;
                float mz = _stickMovement.DirMove.y;
                mv = Vector3.zero;
#if UNITY_EDITOR
                float rotate = Input.GetAxis("Horizontal");
                float move = Input.GetAxis("Vertical");

                mv = transform.forward * move;
                rv = Vector3.up * rotate;
#endif
                mv = mv + transform.forward * mz;

                mv = mv.magnitude < 1 ? mv : mv.normalized;

                mv = mv * Time.deltaTime * _moveSpeed;
                mv.y += Physics.gravity.y * 0.2f;
                rv = (rv + Vector3.up * mx) * Time.deltaTime * _rotateSpeed;

                AnimChange(mv);
                _controller.Move(mv);
                transform.Rotate(rv);
            }
        }
    }

    void AnimChange(Vector3 mv)
    {
        mv.y -= Physics.gravity.y * 0.2f;

        if (mv != Vector3.zero)
            _playerAnim.SetBool("Move", true);
        else
            _playerAnim.SetBool("Move", false);
    }

    public void Select()
    {
        _selectObject = _onObject;
        _selectObject.Select(this);
        _playerAnim.SetBool("Move", false);
        _actionBtn.gameObject.SetActive(false);
    }

    public void SelectEnd()
    {
        _selectObject = null;
        _actionTxt.text = _onObject._selectName;
        _actionBtn.gameObject.SetActive(true);
    }

    public void WalkSound()
    {
        SoundManager.Instance.PlayEffectSound(ESoundName.PlayerMove, transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SelectObject"))
        {
            SelectObject selectObject = other.GetComponent<SelectObject>();
            selectObject.OnPlayerEnter();
            _onObject = selectObject;
            _actionTxt.text = selectObject._selectName;
            _actionBtn.interactable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SelectObject"))
        {
            SelectObject selectObject = other.GetComponent<SelectObject>();
            selectObject.OnPlayerExit();
            _onObject = null;
            _actionTxt.text = "";
            _actionBtn.interactable = false;
        }
    }
}
