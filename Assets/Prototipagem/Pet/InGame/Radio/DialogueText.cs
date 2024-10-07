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
    public string name;

    public LocalizedString localizedQuote;
    [TextArea(1, 20)] public string quote;

    [Header("Events")]
    public DialogueEvent[] eventOnDialogueBoxEnter;
}
