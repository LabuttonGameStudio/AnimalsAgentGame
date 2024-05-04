using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public Sprite Icon;
    public string[] dialogue;
    public string titlename;
    

    private DialogueBasicControl db;
    private bool DialoguePause;

    private void Awake()
    {
        db = FindObjectOfType<DialogueBasicControl>();
    }

    public void ShowDialogueNotPause()
    {
        DialoguePause = false;
        db.Dialogues(Icon,titlename, dialogue);
    }

    public void ShowDialoguePause()
    {
        DialoguePause = true;
        StartCoroutine(ShowDialogueWithPause());
    }

    IEnumerator ShowDialogueWithPause() // teste, nao finalizado
    {
        db.Dialogues(Icon, titlename, dialogue);
        yield return new WaitForSeconds(1.5f); // Aguarda 1,5 segundos antes de permitir pular o texto
       
        // Se o jogador pressionar a tecla E, pule o diálogo
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("tentei skipar");
            db.CloseDialogues();
            yield break; 
        }
    }


}
