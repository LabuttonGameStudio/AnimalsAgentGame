using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BulletManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void UpdateText(string newTitle, string newDescription)
    {
        if (titleText != null && descriptionText != null)
        {
            titleText.text = newTitle;
            descriptionText.text = newDescription;
        }
    }

    #region Tutorial - fase 1
    public void Objetivo1()
    {
        UpdateText("reconhecimento de campo", "Escale a montanha para novas instruções");
    }

    #endregion

    #region Cais - fase 2

    #endregion
}

