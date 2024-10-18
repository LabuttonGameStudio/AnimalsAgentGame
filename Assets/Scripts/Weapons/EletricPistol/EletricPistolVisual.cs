using System.Collections;
using UnityEngine;

public class EletricPistolVisual : MonoBehaviour
{
    public static EletricPistolVisual Instance;

    [SerializeField] private GameObject model;
    [Header("VFX")]
    [SerializeField] private ParticleSystem onFireParticle;
    [SerializeField] private ParticleSystem onFireTrail;
    [SerializeField] private ParticleSystem onReloadParticle;
    [SerializeField] private ParticleSystem ZiumOnomatopeiaParticle;
    [SerializeField] private LineRenderer projectileLineRenderer;
    [Header("Audio")]
    [SerializeField] public Transform audioEmitterTransform;
    [SerializeField] private SoundEmitter onUnchargedFireSoundEmitter;


    float projectileLineRendererStartAlphaValue;
    float projectileLineRendererEndAlphaValue;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        projectileLineRendererStartAlphaValue = projectileLineRenderer.startColor.a;
        projectileLineRendererEndAlphaValue = projectileLineRenderer.endColor.a;
    }
    public void ToggleVisual(bool state)
    {
        model.SetActive(state);
    }
    public void OnUnchargedFire(Vector3 destination)
    {
        ZiumOnomatopeiaParticle.Play();

        onFireParticle.Play();

        //onFireTrail.Play();
        StartProjectileLineRenderer(destination);

        onUnchargedFireSoundEmitter.PlayAudio();
        FPCameraShake.Instance.StartPivotOffsetFixed(0f, Vector3.back / 2, 0.05f, 0.05f);
    }

    private void StartProjectileLineRenderer(Vector3 destination)
    {
        if (projectileLineUpdate_Ref != null) StopCoroutine(projectileLineUpdate_Ref);
        projectileLineUpdate_Ref = StartCoroutine(ProjectileLineUpdate_Coroutine(destination));
    }

    private Coroutine projectileLineUpdate_Ref;
    private IEnumerator ProjectileLineUpdate_Coroutine(Vector3 destination)
    {
        float timer = 0f;
        float duration = 0.2f;

        projectileLineRenderer.enabled = true;
        while (timer < duration)
        {
            projectileLineRenderer.startColor = new Color(projectileLineRenderer.startColor.r, projectileLineRenderer.startColor.g, projectileLineRenderer.startColor.b, projectileLineRendererStartAlphaValue * (1 - timer / duration));
            projectileLineRenderer.endColor = new Color(projectileLineRenderer.endColor.r, projectileLineRenderer.endColor.g, projectileLineRenderer.endColor.b, projectileLineRendererEndAlphaValue * (1 - timer / duration));
            projectileLineRenderer.SetPosition(1, transform.InverseTransformPoint(destination));
            timer += Time.deltaTime;
            yield return null;
        }
        projectileLineRenderer.enabled = false;
        projectileLineRenderer.startColor = new Color(projectileLineRenderer.startColor.r, projectileLineRenderer.startColor.g, projectileLineRenderer.startColor.b, projectileLineRendererStartAlphaValue);
        projectileLineRenderer.endColor = new Color(projectileLineRenderer.endColor.r, projectileLineRenderer.endColor.g, projectileLineRenderer.endColor.b, projectileLineRendererEndAlphaValue);
    }

    private void UpdateUI()
    {

    }

    public void ResetVisuals()
    {

    }
}
