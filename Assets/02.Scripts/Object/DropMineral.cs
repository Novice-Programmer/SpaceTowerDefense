using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMineral : MonoBehaviour
{
    int _mineral = 0;
    float _viewMaxTime = 10f;

    float _timeCheck = 0;

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (transform.position.y < 0)
            {
                GetComponent<Rigidbody>().AddExplosionForce(100.0f, Vector3.up, 10.0f, 100.0f);
            }
            _timeCheck += Time.deltaTime;
            if (_timeCheck >= _viewMaxTime)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void MineralDrop(int mineralValue, Vector3 pos, Vector3 right)
    {
        gameObject.SetActive(true);
        _timeCheck = 0;
        _mineral = mineralValue;
        pos.y += 2f;
        transform.position = pos;
        float rd = Random.Range(-1f, 1f);
        Vector3 dropPos;
        if (rd < 0)
            dropPos = right * -1f;
        else
            dropPos = right * 1f;

        GetComponent<Rigidbody>().AddForce(dropPos.normalized * 5f);
    }

    public void GetMineral(bool endGet = false)
    {
        if (!endGet)
            SoundManager.Instance.PlayEffectSound(ESoundName.GetMineral, transform);
        gameObject.SetActive(false);
        ResourceManager.Instance.SpaceMineralValue = _mineral;
    }
}
