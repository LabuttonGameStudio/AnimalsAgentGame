using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGunVisual : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private Transform firePivot;
    public void ToggleVisual(bool state)
    {
        model.SetActive(state);
    }
    public void ResetVisuals()
    {

    }
}
