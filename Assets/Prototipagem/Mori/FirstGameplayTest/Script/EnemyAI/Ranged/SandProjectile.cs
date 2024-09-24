using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SandProjectile : MonoBehaviour
{
    private float lifeSpanTimer;
    [HideInInspector]public float lifeSpanDuration;
    private Rigidbody rb;
    public static float damageAmount;
    [SerializeField] private DecalProjector bulletDecalProjector;
    private Vector3 direction;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        damageAmount = 30;
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
        if(lifeSpanTimer > lifeSpanDuration)gameObject.SetActive(false);
    }
    public void Fire(Vector3 origin, Vector3 direction)
    {
        transform.position = origin;
        this.direction = direction;
        gameObject.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(new Damage(damageAmount, Damage.DamageType.Slash, false, Vector3.zero));
            bulletDecalProjector.transform.parent = null;
            bulletDecalProjector.enabled = true;
            gameObject.SetActive(false);
        }
        else if (!other.isTrigger)
        {
            bulletDecalProjector.transform.parent = null;
            bulletDecalProjector.enabled = true;
            gameObject.SetActive(false);
        }
    }
}
