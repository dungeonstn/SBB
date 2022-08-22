using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBecameInvisible : MonoBehaviour
{
    private Vector3 startPos;
    private Material blockOriginal;
    public Material inviTransp;
    private bool collided;

    private void Start()
    {
        blockOriginal = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        startPos = transform.localPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collided && collision.collider.CompareTag("Player"))
        {
            collided = true;
            StartCoroutine(Fall());
        }
    }

    IEnumerator Fall()
    {
        float shakeTime = 0;
        Vector3 randPos = transform.localPosition;
        while(shakeTime < 1.5f)
        {
            if(transform.localPosition != randPos)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, randPos, 10 * Time.deltaTime);
            }
            else
            {
                randPos = new Vector3(startPos.x, startPos.y, Random.Range(startPos.z - 0.1f, startPos.z + 0.1f));
            }

            shakeTime += Time.deltaTime;

            yield return null;
        }

        transform.GetChild(0).GetComponent<MeshRenderer>().material = inviTransp;
        transform.GetComponent<BoxCollider>().enabled = false;
        transform.localPosition = startPos;

        float fallTimer = 0;
        while(fallTimer < 3)
        {
            fallTimer += Time.deltaTime;
            yield return null;
        }
        transform.GetChild(0).GetComponent<MeshRenderer>().material = blockOriginal;
        transform.GetComponent<BoxCollider>().enabled = true;
        collided = false;
    }
}
