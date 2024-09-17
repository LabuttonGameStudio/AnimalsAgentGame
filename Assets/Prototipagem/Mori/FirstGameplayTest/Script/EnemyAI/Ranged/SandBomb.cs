using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBomb : MonoBehaviour
{
    public void Explode()
    {
        RaycastHit[] hit = Physics.SphereCastAll(transform.position, 6, Vector3.up);
    }    
}
