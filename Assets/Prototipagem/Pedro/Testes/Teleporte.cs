using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporte : MonoBehaviour
{
    // Destino do teleporte
    public Transform teleportDestination;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que colidiu é o jogador
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entrou no trigger. Teletransportando...");
            // Teletransporta o jogador para o destino
            other.transform.position = teleportDestination.position;
        }
    }
}
