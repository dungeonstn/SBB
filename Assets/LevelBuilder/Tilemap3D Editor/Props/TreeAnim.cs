using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAnim : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(Anim());
    }

    IEnumerator Anim()
    {
        float timer = 0;
        while (timer < 2)
        {
            transform.localEulerAngles += Vector3.right *  -2f * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        while (timer < 2)
        {
            transform.localEulerAngles -= Vector3.right * -2f * Time.deltaTime;
            timer += Time.deltaTime;

            yield return null;
        }

        StartCoroutine(Anim());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
