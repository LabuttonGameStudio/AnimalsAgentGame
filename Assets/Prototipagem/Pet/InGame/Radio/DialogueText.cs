using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DialogueText 
{
    [Header("Infos")]
    public Sprite portrait;
    public string name;
    public string quote;

    [Header("Events")]
    public DialogueEvent[] events;
}
