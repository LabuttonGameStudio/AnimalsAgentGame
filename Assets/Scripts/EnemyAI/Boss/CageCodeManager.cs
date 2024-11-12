using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private CageSegment segment0;
    [SerializeField] private CageSegment segment1;
    [SerializeField] private CageSegment segment2;
    [SerializeField] private CageSegment segment3;

    private CodeSymbolsEnum[] currentCode;
    private int currentCodeIndex;
    private void Awake()
    {
        Instance = this;
        currentCode = new CodeSymbolsEnum[4];
    }
    public void InsertCode(CodeSymbolsEnum code)
    {
        currentCode[currentCodeIndex] = code;
        currentCodeIndex++;
        if (currentCodeIndex >= 4)
        {
            CheckForMatchingPassword();
        }
    }
    public void CheckForMatchingPassword()
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
            for(int y = 0;y<4;y++)
            {

            }
        }
    }
}
