using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class ArmadilloHPControl : MonoBehaviour, IDamageable
{
    [Header("HP Stats")]
    public float maxHp;
    public float currentHp;
    public float currentGreyHp;

    public float currentShield;

    [Space, Header("HP UI")]
    public Slider currentHpSlider;
    public Slider currentGreyHpSlider;
    public Slider currentShieldSlider;
    public Material DamageMaterial;
    public CanvasGroup DamageScreen;
    public CanvasGroup DeathScreen;

    [Space, Header("Invulnerabiliy Time")]
    [System.NonSerialized] public bool isInvulnerable;
    public float invulnerabilityDuration;

    private float DamageStrength;
    private float DamageAmount;
    private Color DamageColor;

    private void Start()
    {
        UpdateHealthBar();
        DamageMaterial.SetFloat("_DamageStrength", 0);
        DamageMaterial.SetFloat("_DamageAmount", 0);
        DamageMaterial.SetColor("_DamageColor", Color.white);
    }
    private void OnDisable()
    {
        DamageMaterial.SetFloat("_DamageStrength", 0);
        DamageMaterial.SetFloat("_DamageAmount", 0);
        DamageMaterial.SetColor("_DamageColor", Color.white);
    }
    public void TakeDamage(Damage damage)
    {
        if (!isInvulnerable)
        {
            if (currentShield > 0)
            {
                if (currentShield >= damage.damageAmount / 2f)
                {
                    currentShield -= damage.damageAmount / 2f;
                    damage.damageAmount = damage.damageAmount/2f;
                }
                else
                {
                    damage.damageAmount -= currentShield/2f;
                    currentShield = 0;
                }
            }
            if (currentHp - damage.damageAmount <= 0)
            {
                ArmadilloPlayerController.Instance.Die();
                StartCoroutine(DeathScreenFade(1f, 1f));
                currentHp = 0;
                UpdateHealthBar();
                isInvulnerable = true;
                return;
            }
            StartInvulnerabilityTimer();
            if (damageScreenFade_Ref != null) StopCoroutine(damageScreenFade_Ref);
            damageScreenFade_Ref = StartCoroutine(DamageScreenFade_Coroutine(1f, 0.25f));
            currentHp -= damage.damageAmount;
            currentGreyHp = damage.damageAmount / 2;
            if (regenGreyHealth_Ref != null) StopCoroutine(regenGreyHealth_Ref);
            regenGreyHealth_Ref = StartCoroutine(RegenGreyHealth_Coroutine());
            UpdateHealthBar();
        }
    }
    public bool OnHeal(float heal)
    {
        if (currentHp >= maxHp)
        {
            return false;
        }
        if (currentHp + currentGreyHp + heal > maxHp)
        {
            currentHp = maxHp;
            currentGreyHp = 0;
            UpdateHealthBar();
            return true;
        }
        currentHp += heal + currentGreyHp;
        currentGreyHp = 0;
        UpdateHealthBar();
        return true;
    }
    public void UpdateHealthBar()
    {
        currentHpSlider.value = currentHp / maxHp;
        currentGreyHpSlider.value = (currentHp + currentGreyHp) / maxHp;
        currentShieldSlider.value = currentShield / maxHp;
    }
    bool IDamageable.isDead()
    {
        return currentHp <= 0;
    }

    private void StartInvulnerabilityTimer()
    {
        if (invulnerabilityTimer_Ref == null) invulnerabilityTimer_Ref = StartCoroutine(InvulnerabilityTimer_Coroutine());
    }
    private Coroutine invulnerabilityTimer_Ref;
    private IEnumerator InvulnerabilityTimer_Coroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
        invulnerabilityTimer_Ref = null;
    }

    private Coroutine damageScreenFade_Ref;

    private Coroutine regenGreyHealth_Ref;
    private IEnumerator RegenGreyHealth_Coroutine()
    {
        yield return new WaitForSeconds(5);
        while (currentGreyHp > 0)
        {
            currentGreyHp--;
            currentGreyHp = Mathf.Max(0,currentGreyHp);
            currentHp++;
            UpdateHealthBar();
            yield return new WaitForSeconds(0.1f);
        }
        regenGreyHealth_Ref = null;
    }
    private IEnumerator DamageScreenFade_Coroutine(float targetAlpha, float fadeDuration)
    {
        DamageStrength = 0f;
        DamageAmount = 0f;
        DamageColor = Color.white;
        DamageMaterial.SetFloat("_DamageStrength", DamageStrength);
        DamageMaterial.SetFloat("_DamageAmount", DamageAmount);
        DamageMaterial.SetColor("_DamageColor", DamageColor);

        float elapsedTime = 0.0f;
        fadeDuration /= 3f;

        // aumenta valores e muda cor para vermelho
        while (elapsedTime < fadeDuration)
        {
            DamageStrength = Mathf.Lerp(0f, 0.2f, elapsedTime / fadeDuration);
            DamageAmount = Mathf.Lerp(0f, 50f, elapsedTime / fadeDuration);
            DamageColor = Color.Lerp(Color.white, new Color(1f, 0.7f, 0.7f), elapsedTime / fadeDuration);

            DamageMaterial.SetFloat("_DamageStrength", DamageStrength);
            DamageMaterial.SetFloat("_DamageAmount", DamageAmount);
            DamageMaterial.SetColor("_DamageColor", DamageColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(fadeDuration);
        elapsedTime = 0.0f;

        // Diminui e troca cor pra branco
        while (elapsedTime < fadeDuration)
        {
            DamageStrength = Mathf.Lerp(0.2f, 0f, elapsedTime / fadeDuration);
            DamageAmount = Mathf.Lerp(50f, 0f, elapsedTime / fadeDuration);
            DamageColor = Color.Lerp(new Color(1f, 0.7f, 0.7f), Color.white, elapsedTime / fadeDuration);

            DamageMaterial.SetFloat("_DamageStrength", DamageStrength);
            DamageMaterial.SetFloat("_DamageAmount", DamageAmount);
            DamageMaterial.SetColor("_DamageColor", DamageColor);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Reseta os valores ao final
        DamageStrength = 0f;
        DamageAmount = 0f;
        DamageColor = Color.white;
        DamageMaterial.SetFloat("_DamageStrength", DamageStrength);
        DamageMaterial.SetFloat("_DamageAmount", DamageAmount);
        DamageMaterial.SetColor("_DamageColor", DamageColor);

        damageScreenFade_Ref = null;
}


IEnumerator DeathScreenFade(float targetAlpha, float fadeDuration)
    {
        float startAlpha = DeathScreen.alpha;
        float elapsedTime = 0.0f;
        ArmadilloPlayerController.Instance.inputControl.TogglePlayerControls(false);
        float timer = 0;
        while (timer < 0.25f)
        {
            Time.timeScale = 1 - timer / 0.25f;
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = 0;
        while (elapsedTime < fadeDuration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            DeathScreen.alpha = newAlpha;

            elapsedTime += Time.unscaledDeltaTime;

            yield return null;
        }

        DeathScreen.alpha = targetAlpha;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
