using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTimeCheck : MonoBehaviour
{
	void OnEnable()
	{
		StartCoroutine("CheckIfAlive");
	}

	IEnumerator CheckIfAlive()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.2f);
			if (!GetComponent<ParticleSystem>().IsAlive(true))
			{
				Destroy(gameObject);
				break;
			}
		}
	}
}
