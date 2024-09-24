using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBomb : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private ParticleSystem explosionParticle;
    [HideInInspector] public float explosionRange=3;
    [HideInInspector] public float damage;
    [SerializeField] SandSlowArea sandSlowArea;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        explosionRange = 3;
        damage = 30;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void Launch(Vector3 startPoint, Vector3 finalPoint)
    {
        transform.position = startPoint;
        gameObject.SetActive(true);
        launchParable_Ref = StartCoroutine(LaunchParable_Coroutine(startPoint, finalPoint));
    }
    Coroutine launchParable_Ref;
    IEnumerator LaunchParable_Coroutine(Vector3 startPoint, Vector3 finalPoint)
    {
        Vector3 deltaDistance = finalPoint - startPoint;
        deltaDistance.y = 0;
        float totalDistance = deltaDistance.magnitude;
        float timer = 0;
        float duration = totalDistance / 20;
        while (timer < duration)
        {
            rb.MovePosition(MathFExtras.Parabola(startPoint, finalPoint, 3, timer / duration));
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (launchParable_Ref != null)
        {
            StopCoroutine(launchParable_Ref);
            launchParable_Ref = null;
            Explode();
            gameObject.SetActive(false);
        }
    }
    public void Explode()
    {
        explosionParticle.transform.position = transform.position;
        explosionParticle.Play();
        if(Physics.Raycast(transform.position,Vector3.down,out RaycastHit groundCast,10, LayerManager.Instance.groundMask,QueryTriggerInteraction.Ignore))
        {
            sandSlowArea.transform.position = groundCast.point;
            sandSlowArea.gameObject.SetActive(true);
        }
        RaycastHit[] hitArray = Physics.SphereCastAll(transform.position, explosionRange, Vector3.up);
        foreach (RaycastHit hit in hitArray)
        {
            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(new Damage(damage, Damage.DamageType.Blunt, false, transform.position));
            }
        }
    }
}
