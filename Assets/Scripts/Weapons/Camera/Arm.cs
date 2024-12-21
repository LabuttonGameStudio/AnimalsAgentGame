using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : MonoBehaviour
{
    public Transform lerpToPos;
    private Vector3 deltaPos;
    [SerializeField] private float velocityOfLerp =3f;
    public void Awake()
    {
        deltaPos = transform.localPosition;
    }
    void Update()
    {

    }
}
