using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixeye.Unity;
public class BombardierBomb : MonoBehaviour
{
    static private Material turnOffMat;
    static private Material turnOnMat;

    private Rigidbody rb;
    [SerializeField] private MeshRenderer meshRenderer;
    [Foldout("Explosion Mechanic", true)]
    [SerializeField] private GameObject explosionArea;

    [Foldout("VFX", true)]
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
        if (turnOffMat == null)
        {
            turnOffMat = new Material(meshRenderer.material);
            turnOffMat.SetInt("_Emissive", 0);
        }
        meshRenderer.sharedMaterial = turnOffMat;
        if (turnOnMat == null)
        {
            turnOnMat = new Material(meshRenderer.material);
            turnOnMat.SetInt("_Emissive", 1);
        }
    }
    private void Start()
    {
        lineRenderer.transform.position = new Vector3(100000, 100000, 100000);
        lineRenderer.positionCount = pointsCount + 1;
        gameObject.SetActive(false);
    }
    public void Enable()
    {
        meshRenderer.sharedMaterial = turnOffMat;
        meshRenderer.transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }
    private void OnCollisionEnter(Collision collision)
    {
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
        float timer = 0;
        float duration = 1.5f;
        float interval = 0.35f;
        while (timer < duration)
        {
            meshRenderer.sharedMaterial = turnOnMat;
            yield return new WaitForSeconds(interval);
            timer += interval;
            interval -= 0.05f;
            interval = Mathf.Max(interval, 0.05f);

            meshRenderer.sharedMaterial = turnOffMat;
            yield return new WaitForSeconds(interval);
            timer += interval;
            interval -= 0.05f;
            interval = Mathf.Max(interval, 0.05f);
        }
        meshRenderer.sharedMaterial = turnOnMat;
        timer = 0;
        duration = 0.25f;
        while (timer < duration)
        {
            meshRenderer.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.25f, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        meshRenderer.transform.localScale = Vector3.one * 1.25f;
        Collider[] objectsHit = Physics.OverlapSphere(transform.position, 4, LayerManager.Instance.enemyAttackMask, QueryTriggerInteraction.Ignore);

        foreach (Collider collider in objectsHit)
        {
            if (collider.TryGetComponent(out IDamageable damageable))
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
