using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CageCodeManager;

[Serializable]
public class CodeSymbol
{
    public CodeSymbolsEnum segmentDigit;
    public CodeBase digitScript;
}
public class CageSegment : MonoBehaviour
{
    [HideInInspector]public bool isUnlocked;
    public CodeSymbol[] segmentPassword;
}
