using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBomb : MonoBehaviour
{
    Rigidbody rb;
    SphereCollider sphereCollider;
    [SerializeField] private ParticleSystem explosionParticle;
    private MeshRenderer meshRenderer;
    [HideInInspector] public float explosionRange = 3;
    [HideInInspector] public float damage;
    [SerializeField] SandSlowArea sandSlowArea;

    [SerializeField] private ParticleSystem buildUpParticle;
    [SerializeField] private ParticleSystemForceField buildUpParticleForceField;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        explosionRange = 3;
        damage = 10;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
    private void Start()
    {
        ToggleFunctions(false);
        ToggleVisual(false);
    }
    public void ToggleVisual(bool state)
    {
        meshRenderer.enabled = state;
    }
    public void ToggleFunctions(bool state)
    {
        rb.isKinematic = !state;
        sphereCollider.enabled = state;
        this.enabled = state;
    }
    public void StartSpawnVFX()
    {
        buildUpParticle.Play();
        buildUpParticleForceField.enabled = true;
    }
    public void StopSpawnVFX()
    {
        buildUpParticle.Stop();
        buildUpParticleForceField.enabled = false;
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
        float height = totalDistance / 8;
        while (timer < duration)
        {
            rb.MovePosition(MathFExtras.Parabola(startPoint, finalPoint, height, timer / duration));
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemies")) return;
        if (launchParable_Ref != null)
        {
            StopCoroutine(launchParable_Ref);
            launchParable_Ref = null;
            Explode();
            ToggleVisual(false);
            ToggleFunctions(false);
        }
    }
    public void Explode()
    {
        explosionParticle.transform.position = transform.position;
        explosionParticle.Play();
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit groundCast, 10, LayerManager.Instance.groundMask, QueryTriggerInteraction.Ignore))
        {
            sandSlowArea.transform.position = groundCast.point;
            sandSlowArea.gameObject.SetActive(true);
        }
        Collider[] collidersInside = Physics.OverlapSphere(transform.position, explosionRange, LayerManager.Instance.enemyAttackMask, QueryTriggerInteraction.Ignore);
        foreach (Collider collider in collidersInside)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemies")) return;
            if (collider.TryGetComponent(out IDamageable damageable))
            {
                Vector3 direction = collider.transform.position - transform.position;
                Physics.Raycast(transform.position, direction.normalized, out RaycastHit raycastHit, direction.magnitude, LayerManager.Instance.activeColliders, QueryTriggerInteraction.Ignore);
                if (raycastHit.collider == collider)
                {
                    damageable.TakeDamage(new Damage(damage, Damage.DamageType.Blunt, false, transform.position));
                }
            }
        }
    }
}
