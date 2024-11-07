using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandProjectile : MonoBehaviour
{
    private float lifeSpanTimer;
    [HideInInspector]public float lifeSpanDuration;
    private Rigidbody rb;
    public static float damageAmount;
    [SerializeField] private ParticleSystem onHitVFX;

    private Vector3 originPoint;
    private Vector3 direction;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        damageAmount = 15;
        lifeSpanDuration = 5;
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + direction);
        lifeSpanTimer += Time.fixedDeltaTime;
        if (lifeSpanTimer > lifeSpanDuration)
        {
            DisableBullet();
        }
    }
    public void Fire(Vector3 origin, Vector3 direction)
    {
        originPoint = origin;
        transform.position = origin;
        transform.LookAt(transform.position + direction);
        this.direction = direction;
        gameObject.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemies")) return;
        if (other.TryGetComponent(out IDamageable damageable))
        {
            onHitVFX.transform.position = other.bounds.ClosestPoint(transform.position - direction * 3);
            onHitVFX.Play();
            damageable.TakeDamage(new Damage(damageAmount, Damage.DamageType.Slash, false, Vector3.zero));
            DisableBullet();
        }
        else if (!other.isTrigger)
        {
            onHitVFX.transform.position = other.bounds.ClosestPoint(transform.position-direction*3);
            onHitVFX.Play();
            DisableBullet();
        }
    }
    private void DisableBullet()
    {
        direction = Vector3.zero;
        lifeSpanTimer = 0;
        gameObject.SetActive(false);
    }
}
