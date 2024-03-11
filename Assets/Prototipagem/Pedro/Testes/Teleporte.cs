using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporte : MonoBehaviour
{
    public Transform startPoint; // Ponto de partida
    public Transform teleportPoint; // Ponto para teletransporte

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jogador entrou no trigger em: " + startPoint.position);
            Debug.Log("Teleportando para: " + teleportPoint.position);

            // Teleporta o jogador para o ponto de teletransporte
            other.transform.position = teleportPoint.position;
        }
    }
}
