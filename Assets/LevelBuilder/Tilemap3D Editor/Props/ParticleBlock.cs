using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBlock : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + transform.up*0.5f, Vector3.one);
    }
}
