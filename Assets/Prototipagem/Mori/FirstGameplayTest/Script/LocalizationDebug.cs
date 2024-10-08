using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationDebug : MonoBehaviour
{
    public Languages currentLanguage;
    public enum Languages
    {
        English,
        Portuguese,
    }
    private void OnValidate()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[(int)currentLanguage];
    }
    private void Start()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[(int)currentLanguage];
    }
}
