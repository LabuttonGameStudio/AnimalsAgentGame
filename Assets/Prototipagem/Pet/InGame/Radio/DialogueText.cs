using System;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class DialogueText 
{
    [Header("Infos")]
    public Sprite portraitBackground;
    public Material portrait;

    public LocalizedString localizedName;

    public LocalizedString localizedQuote;

    [Header("Events")]
    public DialogueEvent[] eventOnDialogueBoxEnter;
}
