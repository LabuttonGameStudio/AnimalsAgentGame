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
            if(scaleLerp_Ref != null)
            {
                StopCoroutine(scaleLerp_Ref);
            }
            if(colorLerp_Ref != null)
            {
                StopCoroutine(scaleLerp_Ref);
            }
        }
        toggleHitMarkerDamage_Ref = StartCoroutine(ToggleHitMarkerDamage_Coroutine());
    }

    public void StartHitMarkerCrit()
    {
        if (toggleHitMarkerCrit_Ref != null)
        {
            StopCoroutine(toggleHitMarkerCrit_Ref);
        }
        toggleHitMarkerCrit_Ref = StartCoroutine(ToggleHitMarkerCrit_Coroutine());
    }

    public void StartHitMarkerLethal()
    {
        if (toggleHitMarkerLethal_Ref != null)
        {
            StopCoroutine(toggleHitMarkerLethal_Ref);
        }
        toggleHitMarkerLethal_Ref = StartCoroutine(ToggleHitMarkerLethal_Coroutine());
    }

    private Coroutine scaleLerp_Ref;
    private Coroutine colorLerp_Ref;

    private Coroutine toggleHitMarkerDamage_Ref;
    private IEnumerator ToggleHitMarkerDamage_Coroutine()
    {
        RectTransform hitMarkerRect = hitMarkerDamage.rectTransform;
        hitMarkerRect.localScale = Vector3.one;
        hitMarkerDamage.color = new Color(hitMarkerDamage.color.r, hitMarkerDamage.color.g, hitMarkerDamage.color.b, 1);
        float duration = 0.5f;
        float fadeInDuration = 0.1f;
        scaleLerp_Ref = Tween.ScaleTransform(this, hitMarkerRect, new Vector3(0.5f, 0.5f, 0.5f), fadeInDuration, Tween.LerpType.Lerp);
        yield return scaleLerp_Ref;
        colorLerp_Ref = Tween.LerpColor(this, hitMarkerDamage, new Color(hitMarkerDamage.color.r, hitMarkerDamage.color.g, hitMarkerDamage.color.b, 0), duration - fadeInDuration);
        scaleLerp_Ref = Tween.ScaleTransform(this, hitMarkerRect, Vector3.zero, duration - fadeInDuration, Tween.LerpType.Lerp);
        yield return scaleLerp_Ref;
        toggleHitMarkerDamage_Ref = null;
    }

    public Coroutine toggleHitMarkerCrit_Ref;
    private IEnumerator ToggleHitMarkerCrit_Coroutine()
    {
        RectTransform hitMarkerRect = hitMarkerCrit.rectTransform;
        hitMarkerCrit.color = new Color(hitMarkerCrit.color.r, hitMarkerCrit.color.g, hitMarkerCrit.color.b, 1);
        crossHair.color = new Color(1.0f, 0.5f, 0.0f);

        hitMarkerRect.localScale = Vector3.zero;

        yield return new WaitForSeconds(0.1f);
        float timer = 0;
        float duration = 0.15f;

        while (timer < duration)
        { 
            yield return Tween.ScaleTransform(this, hitMarkerRect, new Vector3(1.3f, 1.3f, 1.3f), 0.05f / 2, Tween.LerpType.Lerp);
            yield return Tween.ScaleTransform(this, hitMarkerRect, Vector3.one, 0.05f / 2, Tween.LerpType.Lerp);

            hitMarkerCrit.color = new Color(hitMarkerCrit.color.r, hitMarkerCrit.color.g, hitMarkerCrit.color.b, 1 - timer / duration);

            timer += Time.deltaTime;
            yield return null;
        }
        hitMarkerRect.localScale = Vector3.zero;
        hitMarkerCrit.color = new Color(hitMarkerCrit.color.r, hitMarkerCrit.color.g, hitMarkerCrit.color.b, 0);
        crossHair.color = Color.white;
    }

    public Coroutine toggleHitMarkerLethal_Ref;
    private IEnumerator ToggleHitMarkerLethal_Coroutine()
    {
        RectTransform hitMarkerRect = hitMarkerLethal.rectTransform;
        RectTransform MarkerRect = MarkersLethal.rectTransform;
        Quaternion targetRotation = Quaternion.Euler(0, 0, 180);
        hitMarkerRect.localScale = Vector3.zero;

        hitMarkerLethal.color = new Color(hitMarkerLethal.color.r, hitMarkerLethal.color.g, hitMarkerLethal.color.b, 1);
        MarkersLethal.color = new Color(MarkersLethal.color.r, MarkersLethal.color.g, MarkersLethal.color.b, 1);

        yield return new WaitForSeconds(0.1f);
        float timer = 0;
        float duration = 0.25f;

        yield return Tween.ScaleTransform(this, hitMarkerRect, new Vector3(1.3f, 1.3f, 1.3f), 0.3f / 2, Tween.LerpType.Lerp);
        yield return Tween.ScaleTransform(this, hitMarkerRect, Vector3.one, 0.3f / 2, Tween.LerpType.Lerp);
        yield return Tween.RotateTransform(this, MarkerRect, targetRotation, duration, Tween.LerpType.Lerp);
        crossHair.color = Color.magenta;

        while (timer < duration)
        {
            hitMarkerLethal.color = new Color(hitMarkerLethal.color.r, hitMarkerLethal.color.g, hitMarkerLethal.color.b, 1 - timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        hitMarkerRect.localScale = Vector3.zero;
        MarkerRect.localRotation = Quaternion.Euler(0, 0, 0);
        hitMarkerLethal.color = new Color(hitMarkerLethal.color.r, hitMarkerLethal.color.g, hitMarkerLethal.color.b, 0);
        MarkersLethal.color = new Color(hitMarkerLethal.color.r, hitMarkerLethal.color.g, hitMarkerLethal.color.b, 0);
        crossHair.color = Color.white;
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
