using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporte : MonoBehaviour
{
    public Transform startPoint; // Ponto de partida
    public Transform teleportPoint; // Ponto para teletransporte
    public float teleportDelay = 1.0f; // Atraso antes do teletransporte

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportAfterDelay(other.transform));
        }
    }

    private IEnumerator TeleportAfterDelay(Transform playerTransform)
    {
        Debug.Log("Jogador entrou no trigger em: " + startPoint.position);
        Debug.Log("Teleportando para: " + teleportPoint.position);

        // Aguarda o atraso antes de teletransportar o jogador
        yield return new WaitForSeconds(teleportDelay);

        // Teleporta o jogador para o ponto de teletransporte
        playerTransform.position = teleportPoint.position;
    }
}
