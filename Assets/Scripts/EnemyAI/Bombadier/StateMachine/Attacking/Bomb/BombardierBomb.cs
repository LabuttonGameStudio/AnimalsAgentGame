using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombardierBomb : MonoBehaviour
{
    private Rigidbody rb;
    [Foldout("Explosion Mechanic",styled = true)]
    [SerializeField] private GameObject explosionArea;
    [Foldout("VFX", styled = true)]
    [Header("-----Blast Wave-----")]
    public int pointsCount;
    public float maxRadius;
    public float speed;
    public float startWidth;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("-----Blast Particle-----")]
    [SerializeField] private ParticleSystem[] blastParticleSystems;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        lineRenderer.transform.position = new Vector3(100000, 100000, 100000);
        lineRenderer.positionCount = pointsCount + 1;
        gameObject.SetActive(false);
    }
    public void Enable()
    {
        gameObject.SetActive(true);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.rigidbody == null)
        {
            if (explosion_Ref == null) explosion_Ref = StartCoroutine(Explosion_Coroutine());
        }
    }
    private Coroutine explosion_Ref;
    private IEnumerator Explosion_Coroutine()
    {
        rb.isKinematic = true;
        explosionArea.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        Collider[] objectsHit = Physics.OverlapSphere(transform.position, 4,Physics.AllLayers,QueryTriggerInteraction.Ignore);

        foreach(Collider collider in objectsHit)
        {
            if(collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(new Damage(40, Damage.DamageType.Explosive, false, transform.position));
            }
            yield return null;
        }
        explosionArea.SetActive(false);
        ExplodeVisual(transform.position);
        rb.isKinematic = false;
        explosion_Ref = null;
        gameObject.SetActive(false);

    }
    #region VFX
    public void ExplodeVisual(Vector3 position)
    {
        PlayParticle(position);
        PlayBlastWave(position);
    }

    private void PlayBlastWave(Vector3 position)
    {
        lineRenderer.transform.position = position;
        EnemyMasterControl.Instance.StartCoroutine(BlastWave());
    }
    private IEnumerator BlastWave()
    {
        float currentRadius = 0;
        while (currentRadius < maxRadius)
        {
            currentRadius += Time.deltaTime * speed;
            Draw(currentRadius);
            yield return null;
        }
        lineRenderer.transform.position = new Vector3(100000, 100000, 100000);
    }
    private void Draw(float radius)
    {
        float angleBetweenPoints = 360f / pointsCount;
        for (int i = 0; i <= pointsCount; i++)
        {
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0);
            Vector3 position = direction * radius;
            lineRenderer.SetPosition(i, position);
        }
        lineRenderer.widthMultiplier = Mathf.Lerp(0, startWidth, 1 - radius / maxRadius);
    }
    private void PlayParticle(Vector3 position)
    {
        blastParticleSystems[0].transform.parent.position = position;
        for (int i = 0; i < blastParticleSystems.Length; i++)
        {
            blastParticleSystems[i].Play();
        }
    }
    #endregion
}
