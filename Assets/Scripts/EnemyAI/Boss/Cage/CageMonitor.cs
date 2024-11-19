using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SocialPlatforms;
using static CageCodeManager;

public class CageMonitor : MonoBehaviour
{
    [Header("PC")]
    [SerializeField] private Material lockedPcMat;
    [SerializeField] private Material unlockedPcMat;

    private MeshRenderer meshRenderer;

    [Header("Decals")]
    [SerializeField]private DecalProjector[] decals;

    [SerializeField] private Material triangleDecalMat;
    [SerializeField] private Material squareDecalMat;
    [SerializeField] private Material starDecalMat;
    [SerializeField] private Material circleDecalMat;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        ResetDigits();
    }
    public void EnterDigit(int index, CodeSymbolsEnum codeSymbols)
    {
        index--;
        switch (codeSymbols)
        {
            case CodeSymbolsEnum.None:
                StartCoroutine(DigitFade_Coroutine(decals[index], 1, 0));
                break;
            case CodeSymbolsEnum.Triangle:
                decals[index].material = triangleDecalMat;
                StartCoroutine(DigitFade_Coroutine(decals[index], 0, 1));
                break;
            case CodeSymbolsEnum.Square:
                decals[index].material = squareDecalMat;
                StartCoroutine(DigitFade_Coroutine(decals[index], 0, 1));
                break;
            case CodeSymbolsEnum.Circle:
                decals[index].material = circleDecalMat;
                StartCoroutine(DigitFade_Coroutine(decals[index], 0, 1));
                break;
            case CodeSymbolsEnum.Star:
                decals[index].material = starDecalMat;
                StartCoroutine(DigitFade_Coroutine(decals[index], 0, 1));
                break;
        }
    }
    public void ResetDigits()
    {
        foreach(DecalProjector decal in decals)
        {
            decal.fadeFactor = 0;
        }
    }
    public void UnlockScreen()
    {
        meshRenderer.sharedMaterial = unlockedPcMat;
    }
    public void LockScreen()
    {
        meshRenderer.sharedMaterial = lockedPcMat;
    }
    private IEnumerator DigitFade_Coroutine(DecalProjector decal,float startValue,float endValue)
    {
        float timer = 0f;
        float duration=0.25f;
        while (timer<duration)
        {
            decal.fadeFactor = Mathf.Lerp(startValue, endValue, timer/duration);
            timer += Time.deltaTime;
            yield return null;
        }
        decal.fadeFactor = endValue;
    }
}
