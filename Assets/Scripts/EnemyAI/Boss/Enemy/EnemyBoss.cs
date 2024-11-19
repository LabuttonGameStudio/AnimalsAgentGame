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
    [SerializeField] private float maxHp;
    private float currentHp;

    private bool followPlayer;

    [Header("Swing/Circle Attack")]
    [SerializeField] private float rangeActivation;
    [SerializeField] private float attackKnockback;
    [SerializeField] private float attackDamage;

    [Header("Swing/Circle Attack")]
    [SerializeField] private BossPoisonOrb[] poisonOrb;
    [SerializeField] private float throwForce;
    [SerializeField] private Transform firePivot;
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
        if (actionEnum == ActionsEnum.Idle && attack_Ref == null)
        {
            attack_Ref = StartCoroutine(SprayAttack_Coroutine());
        }
    }


    private void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
        currentHp = maxHp;
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
        float duration = 1f;
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
    }

    public IEnumerator SprayAttack_Coroutine()
    {
        followPlayer = false;
        actionEnum = ActionsEnum.Poison;
        animator.SetTrigger("enterSprayMode");
        yield return new WaitForSeconds(1.875f);
        yield return new WaitForSeconds(0.25f);
        //Spray
        poisonOrb[0].transform.position = firePivot.transform.position;
        poisonOrb[0].gameObject.SetActive(true);
        poisonOrb[0].rb.AddForce(firePivot.transform.forward * throwForce, ForceMode.VelocityChange);
        yield return new WaitForSeconds(0.25f);
        //Spray
        poisonOrb[1].transform.position = firePivot.transform.position;
        poisonOrb[1].gameObject.SetActive(true);
        poisonOrb[1].rb.AddForce(firePivot.transform.forward * throwForce, ForceMode.VelocityChange);
        yield return new WaitForSeconds(0.5f);
        //Spray
        poisonOrb[2].transform.position = firePivot.transform.position;
        poisonOrb[2].gameObject.SetActive(true);
        poisonOrb[2].rb.AddForce(firePivot.transform.forward * throwForce, ForceMode.VelocityChange);
        yield return null;
        animator.SetTrigger("exitSprayMode");
        yield return new WaitForSeconds(2f);
        actionEnum = ActionsEnum.Idle;
        attack_Ref = null;
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
