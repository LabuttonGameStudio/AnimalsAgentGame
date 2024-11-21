using Pixeye.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class EletricPistolVisual : MonoBehaviour
{
    public static EletricPistolVisual Instance;

    [SerializeField] private GameObject model;
    [Foldout("VFX", true)]
    [SerializeField] private ParticleSystem onFireParticle;
    [SerializeField] private ParticleSystem onFireTrail;
    [SerializeField] private ParticleSystem onReloadParticle;
    [SerializeField] private ParticleSystem ZiumOnomatopeiaParticle;
    [SerializeField] private LineRenderer projectileLineRenderer;

    [Header("OnHit Particle")]
    [SerializeField] private ParticleSystem onHitParticle;

    [Header("OnHit Decal")]
    [SerializeField] private DecalProjector[] onHitDecals;

    [Header("OnfireLight")]
    [SerializeField] private Light[] onFireLight;
    private float[] onFireLightDefaultValue;

    [Foldout("SFX", true)]
    [SerializeField] public Transform audioEmitterTransform;
    [SerializeField] private SoundEmitter onUnchargedFireSoundEmitter;
    private int currentDecalIndex;


    //Projectile Line Renderer
    float projectileLineRendererStartAlphaValue;
    float projectileLineRendererEndAlphaValue;

    //
    private void Awake()
    {
        Instance = this;
        onFireLightDefaultValue = new float[onFireLight.Length];
        for (int i = 0; i < onFireLight.Length; i++)
        {
            onFireLightDefaultValue[i] = onFireLight[i].intensity;
            onFireLight[i].intensity = 0f;
            onFireLight[i].enabled = false;
        }
    }
    private void Start()
    {
        projectileLineRendererStartAlphaValue = projectileLineRenderer.startColor.a;
        projectileLineRendererEndAlphaValue = projectileLineRenderer.endColor.a;

        foreach (DecalProjector decal in onHitDecals)
        {
            decal.gameObject.SetActive(false);
        }
    }
    public void ToggleVisual(bool state)
    {
        model.SetActive(state);
    }
    public void OnUnchargedFire(Vector3 destination, Vector3 direction, bool hitSomething, bool setDecal)
    {
        ZiumOnomatopeiaParticle.Play();

        onFireParticle.Play();

        //onFireTrail.Play();
        StartProjectileLineRenderer(destination);

        onUnchargedFireSoundEmitter.PlayAudio();
        FPCameraShake.Instance.StartPivotOffsetFixed(0f, Vector3.back / 2, 0.05f, 0.05f);

        if (hitSomething)
        {
            //Decal
            if (setDecal)
            {
                onHitDecals[currentDecalIndex].transform.position = destination + direction * (-0.1f);
                onHitDecals[currentDecalIndex].transform.LookAt(onHitDecals[currentDecalIndex].transform.position + direction);
                onHitDecals[currentDecalIndex].gameObject.SetActive(true);
            }

            currentDecalIndex++;
            if (currentDecalIndex > onHitDecals.Length - 1) currentDecalIndex = 0;

            //Particle 
            onHitParticle.transform.position = destination + direction * (-0.1f);
            onHitParticle.transform.LookAt(onHitParticle.transform.position + (-1 * direction));
            onHitParticle.Play();
        }
        if(onFireCoroutine_Ref !=null)StopCoroutine(onFireCoroutine_Ref);
        onFireCoroutine_Ref = StartCoroutine(OnFireLight_Coroutine());
    }

    private Coroutine onFireCoroutine_Ref;
    private IEnumerator OnFireLight_Coroutine()
    {
        float timer = 0f;
        float duration = 0.15f;
        float[] onFireLightCurrentValue = new float[onFireLight.Length];
        for (int i = 0; i < onFireLight.Length; i++)
        {
            onFireLightCurrentValue[i] = onFireLight[i].intensity;
            onFireLight[i].enabled = true;
        }
        while (timer < duration)
        {
            for (int i = 0; i < onFireLight.Length; i++)
            {
                onFireLight[i].intensity = Mathf.Lerp(onFireLightCurrentValue[i], onFireLightDefaultValue[i],timer/duration);
            }
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        timer = 0f;
        while (timer < duration)
        {
            for (int i = 0; i < onFireLight.Length; i++)
            {
                onFireLight[i].intensity = Mathf.Lerp(onFireLightDefaultValue[i],0, timer / duration);
            }
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        for (int i = 0; i < onFireLight.Length; i++) onFireLight[i].enabled = false;
        onFireCoroutine_Ref = null;
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

    public void UpdateUI(int currentAmount, int totalAmount, int reserveAmount,int reserveTotal)
    {
        Slider ammoSlider = ArmadilloPlayerController.Instance.weaponControl.Weaponammoslider;
        ammoSlider.value = (float)currentAmount / (float)totalAmount;
        ammoSlider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.red, Color.white, ammoSlider.value);
        ArmadilloPlayerController.Instance.weaponControl.currentWeaponUI.text = " x" + Mathf.Round(ammoSlider.value * 100).ToString() + "%";

        ChangeWeaponUI.Instance.weaponUIElements[0].ammoReserveSlider.value = (float)reserveAmount / (float)reserveTotal;
        ChangeWeaponUI.Instance.weaponUIElements[0].ammoReserveText.text = Mathf.Round((float)reserveAmount / (float)reserveTotal * 100).ToString() + "%";

    }

    public void ToggleReload(bool toggle)
    {
        if (toggle) onReloadParticle.Play();
        else onReloadParticle.Stop();
    }

    public void ResetVisuals()
    {

    }

    public void OnBodyShot()
    {
        ArmadilloUIControl.Instance.StartHitMarkerDamage();
    }
    public void OnHeadshotShot()
    {
        ArmadilloUIControl.Instance.StartHitMarkerCrit();
    }
    public void OnLetalShot()
    {
        ArmadilloUIControl.Instance.StartHitMarkerLethal();
    }
}
