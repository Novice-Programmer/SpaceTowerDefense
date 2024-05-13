using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EDoorType
{
    Door1,
    Door2,
    Door3
}

public class LobbyDoor : MonoBehaviour
{
    [SerializeField] EDoorType _doorType = EDoorType.Door1;
    Animator _doorAnim;

    private void Awake()
    {
        _doorAnim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _doorAnim.SetBool("Enter", true);
            DoorSound();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _doorAnim.SetBool("Enter", false);
            DoorSound();
        }
    }

    public void DoorSound()
    {
        if(_doorType == EDoorType.Door1)
        {
            SoundManager.Instance.PlayEffectSound(ESoundName.DoorOpen1, transform);
        }
        else if(_doorType == EDoorType.Door2)
        {
            SoundManager.Instance.PlayEffectSound(ESoundName.DoorOpen2, transform);
        }
        else
        {
            SoundManager.Instance.PlayEffectSound(ESoundName.DoorOpen3, transform);
        }
    }
}
