using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour
{
    [Header("COMPONENTES")]
    public Sprite Icon;
    public string titlename;

    [Header("TEXT")]
    public string[] dialogue;

    [Header("EVENTS")]
    public UnityEvent startDialogue;
    public UnityEvent endDialogue;

    private DialogueBasicControl db;
    private bool CheckedtypingEnd = false;

    private void Awake()
    {
        db = FindObjectOfType<DialogueBasicControl>();

    }

    public void Update()
    {
        if (db.typingEnd && !CheckedtypingEnd)
        {
            db.EndDialogues(endDialogue);
            CheckedtypingEnd = true;
        }
    }

    public void ShowDialogueNotPause()
    {
        db.StartDialogues(Icon, titlename, dialogue, startDialogue);
        db.Skip.enabled = false;
    }

    public void ShowDialoguePause()
    {
        db.StartDialogues(Icon, titlename, dialogue, startDialogue);
        StartCoroutine(ShowDialogueWithPause());

    }

    //permite pular dialogo
    IEnumerator ShowDialogueWithPause() 
    {
        float elapsedTime = 0f;
        bool canSkip = false;
        db.Skip.enabled = false;
        CheckedtypingEnd = false;

        // Espera 1,5 segundos antes de permitir pular o dialogo
        while (elapsedTime < 1.5f)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= 1.5f)
            {
                // habilita a informacao que pode pular
                db.Skip.enabled = true;
                Debug.Log("Deu o tempo para skipar");

                // Se o jogador pressionar a tecla E fecha o dialogo
                if (Input.GetKeyDown(KeyCode.E))
                {
                    canSkip = true;
                    Debug.Log("quebra do loop while");
                    break;
                }
            }

            yield return null;
        }

        // Se o jogador nao pressionar a tecla E dentro do tempo permitido isso permite que ele consiga fechar a qualquer momento dps de 1,5 seg
        if (!canSkip)
        {
            while (true)
            {
                // Se o jogador pressionar a tecla E fecha o diálogo
                if (Input.GetKeyDown(KeyCode.E))
                {
                    break;
                }
                yield return null;
            }
        }

        // Fechar o diálogo
        db.CloseDialogues();
        db.EndDialogues(endDialogue);

    }

}
