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
        if (lifeSpanTimer > lifeSpanDuration)
        {
            DisableBullet();
        }
    }
    public void Fire(Vector3 origin, Vector3 direction)
    {
        ResetDecal();
        transform.position = origin;
        transform.LookAt(transform.position + direction);
        this.direction = direction;
        gameObject.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        Vector3 hitPosition = transform.position;
        if (other.TryGetComponent(out IDamageable damageable))
        {
            Debug.Log(other.gameObject.name);
            damageable.TakeDamage(new Damage(damageAmount, Damage.DamageType.Slash, false, Vector3.zero));
            DisableBullet();
        }
        else if (!other.isTrigger)
        {
            SetDecal(hitPosition);
            DisableBullet();
        }
    }
    private void SetDecal(Vector3 position)
    {
        bulletDecalProjector.transform.parent = null;
        bulletDecalProjector.enabled = true;
    }
    private void ResetDecal()
    {
        bulletDecalProjector.transform.parent = transform;
        bulletDecalProjector.transform.localPosition = Vector3.zero;
        bulletDecalProjector.enabled = false;
    }
    private void DisableBullet()
    {
        direction = Vector3.zero;
        lifeSpanTimer = 0;
        gameObject.SetActive(false);
    }
}
