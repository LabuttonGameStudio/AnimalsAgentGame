using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMelee : MonoBehaviour,IDamageable
{
    [SerializeField]int currentHp = 50;
    private NavMeshAgent navMeshAgent;

    [SerializeField]List<Transform> aiPathList;

    private int currentAiPathID;
    private bool isInReversePath;
    public void TakeDamage(int damageAmount)
    {
        currentHp -= damageAmount;
        Debug.Log("HP="+currentHp+"| Damage taken="+damageAmount);
        if(currentHp <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    public void Start()
    {
        MoveToNextPosition();
    }
    private void FixedUpdate()
    {
        if(navMeshAgent.remainingDistance<0.1f)
        {
            MoveToNextPosition();
        }
    }
    private void MoveToNextPosition()
    {
        if (aiPathList.Count <= 0) return;
        if(!isInReversePath)
        {
            if (currentAiPathID + 1 > (aiPathList.Count - 1))
            {
                isInReversePath = true;
            }
        }
        else
        {
            if (currentAiPathID - 1 < 0)
            {
                isInReversePath = false;
            }
        }
        if(!isInReversePath) currentAiPathID += 1;
        else currentAiPathID -= 1;
        navMeshAgent.destination = aiPathList[currentAiPathID].position;
    }
}
