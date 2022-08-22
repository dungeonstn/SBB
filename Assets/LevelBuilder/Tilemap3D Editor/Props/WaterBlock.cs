using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBlock : MonoBehaviour
{
    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
        //StartCoroutine(Float());

    }

    private void Update()
    {
        float offset = Time.time * 0.1f;
        transform.GetChild(0).GetComponent<Renderer>().material.SetTextureOffset("_MainTex", Vector2.one + new Vector2(0, -offset));
    }

    IEnumerator Float()
    {
        Vector3 randPos = startPos;
        randPos.y = Random.Range(transform.localPosition.y-0.1f, transform.localPosition.y+0.1f);
        while (transform.localPosition != randPos)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, randPos, 0.07f * Time.deltaTime);
            yield return null;
        }
        while (transform.localPosition != startPos)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, startPos, 0.07f * Time.deltaTime);
            yield return null;
        }

        StartCoroutine(Float());
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
