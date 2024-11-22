using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmadilloUIControl : MonoBehaviour
{
    public static ArmadilloUIControl Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] private Image crossHair;
    [SerializeField] private Image hitMarkerDamage;
    [SerializeField] private Image hitMarkerCrit;
    [SerializeField] private Image hitMarkerLethal;
    [SerializeField] private Image MarkersLethal;

    private enum HitmarkerEnum
    {
        None,
        DamageOnly,
        Critical,
        Letal
    }
    private HitmarkerEnum currentPlayingHitmarker;
    private void Start()
    {
        hitMarkerDamage.color = new Color(hitMarkerDamage.color.r, hitMarkerDamage.color.g, hitMarkerDamage.color.b, 0);
        hitMarkerCrit.color = new Color(hitMarkerCrit.color.r, hitMarkerCrit.color.g, hitMarkerCrit.color.b, 0);
        hitMarkerLethal.color = new Color(hitMarkerLethal.color.r, hitMarkerLethal.color.g, hitMarkerLethal.color.b, 0);
        MarkersLethal.color = new Color(MarkersLethal.color.r, MarkersLethal.color.g, MarkersLethal.color.b, 0);
    }
    #region EletricPistol
    public void StartHitMarkerDamage()
    {
        if (toggleHitMarkerDamage_Ref != null)
        {
            StopCoroutine(toggleHitMarkerDamage_Ref);
        }
        OnHitmarkerOverlap(HitmarkerEnum.DamageOnly);
        toggleHitMarkerDamage_Ref = StartCoroutine(ToggleHitMarkerDamage_Coroutine());
    }

    public void StartHitMarkerCrit()
    {
        if (toggleHitMarkerCrit_Ref != null)
        {
            StopCoroutine(toggleHitMarkerCrit_Ref);
        }
        OnHitmarkerOverlap(HitmarkerEnum.Critical);
        toggleHitMarkerCrit_Ref = StartCoroutine(ToggleHitMarkerCrit_Coroutine());
    }

    public void StartHitMarkerLethal()
    {
        if (toggleHitMarkerLethal_Ref != null)
        {
            StopCoroutine(toggleHitMarkerLethal_Ref);
        }
        OnHitmarkerOverlap(HitmarkerEnum.Letal);
        toggleHitMarkerLethal_Ref = StartCoroutine(ToggleHitMarkerLethal_Coroutine());
    }

    private Coroutine scaleLerp_Ref;
    private Coroutine rotateLerp_Ref;
    private Coroutine colorLerp_Ref;

    private Coroutine toggleHitMarkerDamage_Ref;
    private IEnumerator ToggleHitMarkerDamage_Coroutine()
    {
        currentPlayingHitmarker = HitmarkerEnum.DamageOnly;
        RectTransform hitMarkerRect = hitMarkerDamage.rectTransform;
        hitMarkerRect.localScale = Vector3.one;
        hitMarkerDamage.color = new Color(hitMarkerDamage.color.r, hitMarkerDamage.color.g, hitMarkerDamage.color.b, 1);
        float duration = 0.5f;
        float stayDuration = 0.1f;
        yield return new WaitForSeconds(stayDuration);
        colorLerp_Ref = Tween.LerpColor(this, hitMarkerDamage, new Color(hitMarkerDamage.color.r, hitMarkerDamage.color.g, hitMarkerDamage.color.b, 0), duration - stayDuration);
        scaleLerp_Ref = Tween.ScaleTransform(this, hitMarkerRect, Vector3.zero, duration - stayDuration, Tween.LerpType.Lerp);
        yield return scaleLerp_Ref;
        toggleHitMarkerDamage_Ref = null;
        currentPlayingHitmarker = HitmarkerEnum.None;
    }

    public Coroutine toggleHitMarkerCrit_Ref;
    private IEnumerator ToggleHitMarkerCrit_Coroutine()
    {
        currentPlayingHitmarker = HitmarkerEnum.Critical;
        RectTransform hitMarkerRect = hitMarkerCrit.rectTransform;
        hitMarkerCrit.color = new Color(hitMarkerCrit.color.r, hitMarkerCrit.color.g, hitMarkerCrit.color.b, 1);
        crossHair.color = new Color(1.0f, 0.0f, 0.0f);

        hitMarkerRect.localScale = Vector3.one;

        float duration = 0.5f;
        float stayDuration = 0.25f;
        yield return new WaitForSeconds(stayDuration);
        scaleLerp_Ref = Tween.ScaleTransform(this, hitMarkerRect, Vector3.zero, duration - stayDuration, Tween.LerpType.Lerp);
        colorLerp_Ref = Tween.LerpColor(this, hitMarkerCrit, new Color(hitMarkerCrit.color.r, hitMarkerCrit.color.g, hitMarkerCrit.color.b, 0), duration - stayDuration);
        yield return scaleLerp_Ref;
        crossHair.color = Color.white;
        currentPlayingHitmarker = HitmarkerEnum.None;
    }

    public Coroutine toggleHitMarkerLethal_Ref;
    private IEnumerator ToggleHitMarkerLethal_Coroutine()
    {
        RectTransform hitMarkerRect = hitMarkerLethal.rectTransform;
        RectTransform MarkerRect = MarkersLethal.rectTransform;
        Quaternion targetRotation = Quaternion.Euler(MarkerRect.rotation.eulerAngles + new Vector3(0, 0, 180));
        hitMarkerRect.localScale = Vector3.one;

        hitMarkerLethal.color = new Color(hitMarkerLethal.color.r, hitMarkerLethal.color.g, hitMarkerLethal.color.b, 1);
        MarkersLethal.color = new Color(MarkersLethal.color.r, MarkersLethal.color.g, MarkersLethal.color.b, 1);
        crossHair.color = Color.magenta;

        float duration = 0.6f;
        float stayDuration = 0.3f;

        yield return new WaitForSeconds(stayDuration);
        scaleLerp_Ref = Tween.ScaleTransform(this, hitMarkerRect, Vector3.zero, duration - stayDuration, Tween.LerpType.Lerp);
        rotateLerp_Ref = Tween.RotateTransform(this, MarkerRect, targetRotation, duration - stayDuration, Tween.LerpType.Lerp);
        colorLerp_Ref = Tween.LerpColor(this, hitMarkerLethal, new Color(hitMarkerLethal.color.r, hitMarkerLethal.color.g, hitMarkerLethal.color.b, 0), duration - stayDuration);
        yield return colorLerp_Ref;

        MarkerRect.rotation = Quaternion.Euler(0, 0, 0);
        hitMarkerLethal.color = new Color(hitMarkerLethal.color.r, hitMarkerLethal.color.g, hitMarkerLethal.color.b, 0);
        MarkersLethal.color = new Color(hitMarkerLethal.color.r, hitMarkerLethal.color.g, hitMarkerLethal.color.b, 0);
        crossHair.color = Color.white;
    }

    private void OnHitmarkerOverlap(HitmarkerEnum newHitmarker)
    {
        if (scaleLerp_Ref != null)
        {
            StopCoroutine(scaleLerp_Ref);
        }
        if (colorLerp_Ref != null)
        {
            StopCoroutine(colorLerp_Ref);
        }
        if(rotateLerp_Ref != null)
        {
            StopCoroutine(rotateLerp_Ref);
        }
        if (newHitmarker != currentPlayingHitmarker)
        {
            switch (currentPlayingHitmarker)
            {
                case HitmarkerEnum.DamageOnly:
                    hitMarkerDamage.rectTransform.localScale = Vector3.zero;
                    hitMarkerDamage.color = new Color(hitMarkerDamage.color.r, hitMarkerDamage.color.g, hitMarkerDamage.color.b, 0);
                    break;
                case HitmarkerEnum.Critical:
                    crossHair.color = Color.white;
                    hitMarkerCrit.rectTransform.localScale = Vector3.zero;
                    hitMarkerCrit.color = new Color(hitMarkerCrit.color.r, hitMarkerCrit.color.g, hitMarkerCrit.color.b, 0);
                    break;
                case HitmarkerEnum.Letal:
                    crossHair.color = Color.white;
                    hitMarkerLethal.rectTransform.localScale = Vector3.zero;
                    MarkersLethal.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                    hitMarkerLethal.color = new Color(hitMarkerLethal.color.r, hitMarkerLethal.color.g, hitMarkerLethal.color.b, 0);
                    MarkersLethal.color = new Color(hitMarkerLethal.color.r, hitMarkerLethal.color.g, hitMarkerLethal.color.b, 0);
                    break;
            }
        }
    }

    #endregion

    #region Interaction
    [Space, Header("HUD")]
    public CanvasGroup interactHUD;
    public TextMeshProUGUI interactItemName;
    public TextMeshProUGUI interactKeybind;
    public TextMeshProUGUI interactDescription;
    #endregion
}
