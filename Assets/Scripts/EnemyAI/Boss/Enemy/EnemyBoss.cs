using Pixeye.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    public static EnemyBoss Instance;
    [Foldout("Components", true)]
    public BossDamageableHitbox[] damageableHitboxes;
    private Animator animator;

    [Foldout("Variables", true)]
    [SerializeField] private int totalBatteries;
    private int currentBatteries;

    private bool followPlayer;

    [Header("Swing/Circle Attack")]
    [SerializeField] private float rangeActivation;
    [SerializeField] private float attackKnockback;
    [SerializeField] private float attackDamage;
    private Vector3 circleAttackStartPos;
    private Vector3 circleAttackFinalPos;

    [Header("Poison/Spray Attack")]
    [SerializeField] private BossPoisonOrb[] poisonOrb;
    [SerializeField] private float minThrowForce;
    [SerializeField] private float maxThrowForce;
    [SerializeField] private Transform firePivot;
    [SerializeField] private RotateOverTime rotateOnAttack;
    public enum ActionsEnum
    {
        Idle,
        Enter,
        Swing,
        Poison,
        Die
    }
    [HideInInspector]public ActionsEnum actionEnum;

    public void OnBatteryDestroyed()
    {
        currentBatteries--;
        if (currentBatteries <= 0)
        {
            Die();
        }
        else
        {
            if (actionEnum == ActionsEnum.Idle && attack_Ref == null)
            {
                attack_Ref = StartCoroutine(SprayAttack_Coroutine());
            }
            else
            {
                switch (actionEnum)
                {
                    case ActionsEnum.Swing:
                        StartCoroutine(CircleAttackToSprayAttack_Coroutine());
                        break;
                }
            }
        }
    }
    private void Die()
    {
        StopAllCoroutines();
        animator.SetTrigger("isDefeated");
        BossManager.Instance.onBossDeath.Invoke();
    }
    private void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
        currentBatteries = totalBatteries;
        followPlayer = true;
    }
    public void FixedUpdate()
    {
        Vector3 playerPos = ArmadilloPlayerController.Instance.movementControl.rb.position;
        if (followPlayer)
        {
            transform.LookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z));
        }
        if (Vector2.Distance(new Vector2(playerPos.x, playerPos.z), new Vector2(transform.position.x, transform.position.z)) < rangeActivation)
        {
            if (actionEnum == ActionsEnum.Idle && attack_Ref == null)
            {
                attack_Ref = StartCoroutine(CircleAttack_Coroutine());
            }
        }
    }
    public void ActivatePoisonAttack()
    {
        if (attack_Ref == null)
        {
            attack_Ref = StartCoroutine(SprayAttack_Coroutine());
        }
    }
    private Coroutine attack_Ref;
    public IEnumerator CircleAttack_Coroutine()
    {
        followPlayer = false;
        animator.SetTrigger("enterSwingMode");
        float timer = 0;
        float duration = 0.8332f;
        circleAttackStartPos = transform.parent.position;
        circleAttackFinalPos = circleAttackStartPos - Vector3.up * 3.5f;
        Vector3 startPos = transform.parent.position;
        Vector3 finalPos = startPos - Vector3.up * 3.5f;
        while (timer < duration)
        {
            transform.parent.position = Vector3.Slerp(startPos, finalPos, timer / duration);
            transform.localPosition = Vector3.zero;
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        transform.localPosition = Vector3.zero;
        transform.parent.position = finalPos;

        yield return new WaitForSeconds(0.25f);

        actionEnum = ActionsEnum.Swing;
        duration = 1f;
        Vector3 startRot = transform.localRotation.eulerAngles;
        Vector3 finalRot = transform.localRotation.eulerAngles + Vector3.up * 360;
        while (timer < duration)
        {
            transform.localRotation = Quaternion.Euler(Vector3.Lerp(startRot, finalRot, timer / duration));
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        transform.localRotation = Quaternion.Euler(finalRot);

        animator.SetTrigger("exitSwingMode");
        duration = 2f;
        while (timer < duration)
        {
            transform.parent.position = Vector3.Slerp(finalPos, startPos, timer / duration);
            transform.localPosition = Vector3.zero;
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        transform.localPosition = Vector3.zero;
        transform.parent.position = startPos;
        actionEnum = ActionsEnum.Idle;
        attack_Ref = null;
        followPlayer = true;
    }

    public IEnumerator CircleAttackToSprayAttack_Coroutine()
    {
        StopCoroutine(attack_Ref);
        float timer = 0;
        float duration = 0.2f;
        Vector3 startPos = transform.parent.position;
        Vector3 finalPos = circleAttackStartPos;
        attack_Ref = StartCoroutine(SprayAttack_Coroutine());
        while (timer < duration)
        {
            transform.parent.position = Vector3.Slerp(startPos, finalPos, timer / duration);
            transform.localPosition = Vector3.zero;
            timer += Time.deltaTime;
            yield return null;
        }
        transform.parent.position = finalPos;
        transform.localPosition = Vector3.zero;
    }
    public IEnumerator SprayAttack_Coroutine()
    {
        followPlayer = false;
        actionEnum = ActionsEnum.Poison;
        animator.SetTrigger("enterSprayMode");
        rotateOnAttack.enabled = true;
        float addedTime = 0f;
        float randomTime = Random.Range(0f, 0.05f);
        addedTime += randomTime;
        yield return new WaitForSeconds(0.2f+ randomTime);
        //Time 0.2-0.25f
        //Spray
        poisonOrb[0].transform.position = firePivot.transform.position;
        poisonOrb[0].OnFire();
        poisonOrb[0].gameObject.SetActive(true);
        poisonOrb[0].rb.AddForce(firePivot.transform.forward * Random.Range(minThrowForce,maxThrowForce), ForceMode.VelocityChange);


        randomTime = Random.Range(0f, 0.05f);
        yield return new WaitForSeconds(0.2f+ randomTime - addedTime);
        addedTime += randomTime;
        //Time 0.4-0.5f
        //Spray
        poisonOrb[1].transform.position = firePivot.transform.position;
        poisonOrb[1].OnFire();
        poisonOrb[1].gameObject.SetActive(true);
        poisonOrb[1].rb.AddForce(firePivot.transform.forward * Random.Range(minThrowForce, maxThrowForce), ForceMode.VelocityChange);


        randomTime = Random.Range(0f, 0.05f);
        yield return new WaitForSeconds(0.45f+ randomTime - addedTime);
        addedTime += randomTime;
        //Time 0.85-1.0f
        //Spray
        poisonOrb[2].transform.position = firePivot.transform.position;
        poisonOrb[2].OnFire();
        poisonOrb[2].gameObject.SetActive(true);
        poisonOrb[2].rb.AddForce(firePivot.transform.forward * Random.Range(minThrowForce, maxThrowForce), ForceMode.VelocityChange);


        randomTime = Random.Range(0f, 0.1f);
        yield return new WaitForSeconds(0.8f + randomTime - addedTime);
        addedTime += randomTime;
        //Time 1.65-1.75f
        //Spray
        poisonOrb[3].transform.position = firePivot.transform.position;
        poisonOrb[3].OnFire();
        poisonOrb[3].gameObject.SetActive(true);
        poisonOrb[3].rb.AddForce(firePivot.transform.forward * Random.Range(minThrowForce, maxThrowForce), ForceMode.VelocityChange);


        randomTime = Random.Range(0f, 0.125f);
        yield return new WaitForSeconds(0.225f + randomTime - addedTime);
        addedTime += randomTime;
        //Time 1.875-2f
        //Spray
        poisonOrb[4].transform.position = firePivot.transform.position;
        poisonOrb[4].OnFire();
        poisonOrb[4].gameObject.SetActive(true);
        poisonOrb[4].rb.AddForce(firePivot.transform.forward * Random.Range(minThrowForce, maxThrowForce), ForceMode.VelocityChange);

        yield return null;
        rotateOnAttack.enabled = false;
        animator.SetTrigger("exitSprayMode");
        yield return new WaitForSeconds(2f);
        actionEnum = ActionsEnum.Idle;
        attack_Ref = null;
        followPlayer = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        switch(actionEnum)
        {
            case ActionsEnum.Swing:
                ArmadilloPlayerController player = ArmadilloPlayerController.Instance;
                Vector3 knockback = player.movementControl.rb.position - collision.collider.transform.position;
                knockback = ((player.movementControl.rb.position - transform.position) * 0.5f) + (knockback*0.5f);
                knockback.y = 0;
                knockback = knockback.normalized + Vector3.up * 0.25f;
                player.movementControl.rb.AddForce(knockback * attackKnockback, ForceMode.VelocityChange);
                player.hpControl.TakeDamage(new Damage(attackDamage, Damage.DamageType.Blunt, false, transform.position));
                break;
        }
    }
}
