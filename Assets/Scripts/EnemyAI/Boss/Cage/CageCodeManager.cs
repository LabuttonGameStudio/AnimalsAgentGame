using Pixeye.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
public class CageCodeManager : MonoBehaviour
{
    public static CageCodeManager Instance;
    public enum CodeSymbolsEnum
    {
        None,
        Triangle,
        Square,
        Star,
        Circle
    }
    [Foldout("Components", true)]
    [SerializeField] private CageSegment segment0;
    [SerializeField] private CageSegment segment1;
    [SerializeField] private CageSegment segment2;
    [SerializeField] private CageSegment segment3;

    private int segmentsOpen;
    private CodeSymbolsEnum[] currentCode;
    private int currentCodeIndex;

    [SerializeField] private CageMonitor monitor;

    [SerializeField] private Animator cageAnimator;

    private bool isUnlockingSegment;

    [Foldout("Mechanics", true)]
    public UnityEvent onFirstSegmentCleared;
    public UnityEvent onSecondSegmentCleared;
    public UnityEvent onThirdSegmentCleared;
    public UnityEvent onFourthSegmentCleared;
    private void Awake()
    {
        Instance = this;
        currentCode = new CodeSymbolsEnum[4];
    }
    public void InsertCode(CodeSymbolsEnum code)
    {
        if (isUnlockingSegment) return;
        currentCode[currentCodeIndex] = code;
        currentCodeIndex++;
        if (currentCodeIndex >= 4)
        {
            CheckForMatchingPassword();
            currentCode = new CodeSymbolsEnum[4];
            currentCodeIndex = 0;
        }
    }
    /// <summary>
    /// Triangle=1,Square=2,Star=3,Circle=4
    /// </summary>
    /// <param name="code"></param>
    public void InsertCode(int code)
    {
        if (isUnlockingSegment) return;
        currentCode[currentCodeIndex] = (CodeSymbolsEnum)code;
        currentCodeIndex++;
        FPCameraShake.StartShake(0.1f, 0.1f, 2);
        if (currentCodeIndex >= 4)
        {
            monitor.ResetDigits();
            CheckForMatchingPassword();
            currentCode = new CodeSymbolsEnum[4];
            currentCodeIndex = 0;
        }
        else
        {
            monitor.EnterDigit(currentCodeIndex, (CodeSymbolsEnum)code);
        }
    }
    private void CheckForMatchingPassword()
    {
        for (int i = 0; i < 4; i++)
        {
            CageSegment segment;
            switch (i)
            {
                case 0:
                default:
                    segment = segment0;
                    break;
                case 1:
                    segment = segment1;
                    break;
                case 2:
                    segment = segment2;
                    break;
                case 3:
                    segment = segment3;
                    break;
            }
            //Check Password
            bool matchCode = true;
            for (int y = 0; y < 4; y++)
            {
                if (segment.segmentPassword[y].segmentDigit != currentCode[y])
                {
                    matchCode = false;
                    break;
                }
            }
            if (matchCode)
            {
                OpenSegment(segment, i);
                break;
            }
        }
    }
    public void OpenSegment(CageSegment cageSegment, int segmentIndex)
    {
        foreach (CodeSymbol codeSymbol in cageSegment.segmentPassword)
        {
            codeSymbol.digitScript.ToggleLights(false);
        }
        switch (segmentsOpen)
        {
            case 0:
                onFirstSegmentCleared.Invoke();
                monitor.UnlockScreen(1);
                break;
            case 1:
                onSecondSegmentCleared.Invoke();
                monitor.UnlockScreen(2);
                break;
            case 2:
                onThirdSegmentCleared.Invoke();
                monitor.UnlockScreen(3);
                break;
            case 3:
                onFourthSegmentCleared.Invoke();
                monitor.UnlockScreen(4);
                break;
        }
        cageAnimator.SetBool("hook" + (segmentIndex + 1).ToString() + "IsOpen", true);
        unlockScreen_Ref = StartCoroutine(UnlockScreen_Coroutine());
        cageSegment.isUnlocked = true;
        CheckIfAllSegmentsAreOpen();
        segmentsOpen++;
    }
    private Coroutine unlockScreen_Ref;
    private IEnumerator UnlockScreen_Coroutine()
    {
        isUnlockingSegment = true;
        FPCameraShake.StartShake(3, 0.2f, 8);
        yield return new WaitForSeconds(3f);
        isUnlockingSegment = false;
        unlockScreen_Ref = null;
    }
    private void CheckIfAllSegmentsAreOpen()
    {
        if (segment0.isUnlocked && segment1.isUnlocked && segment2.isUnlocked && segment3.isUnlocked)
        {
            BossManager.Instance.ChangeToSecondPhase();
        }
    }
}
