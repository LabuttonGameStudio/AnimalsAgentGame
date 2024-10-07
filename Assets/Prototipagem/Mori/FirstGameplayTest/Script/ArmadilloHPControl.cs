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

    [Space, Header("HP UI")]
    public Slider currentHpSlider;
    public Slider currentGreyHpSlider;
    public CanvasGroup DamageScreen;
    public CanvasGroup DeathScreen;

    [Space, Header("Invulnerabiliy Time")]
    [System.NonSerialized] public bool isInvulnerable;
    public float invulnerabilityDuration;

    public void TakeDamage(Damage damage)
    {
        if (!isInvulnerable)
        {
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
    }
    private void Start()
    {
        UpdateHealthBar();
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
    private IEnumerator DamageScreenFade_Coroutine(float targetAlpha, float fadeDuration)
    {
        float startAlpha = DamageScreen.alpha;
        float elapsedTime = 0.0f;

        // Faz a tela de dano aumentar o alpha
        while (elapsedTime < fadeDuration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            DamageScreen.alpha = newAlpha;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        DamageScreen.alpha = targetAlpha;

        // Espera um tempo antes de come�ar a diminuir o alpha
        yield return new WaitForSeconds(0.5f);

        elapsedTime = 0.0f;

        // Faz a tela de dano diminuir o alpha
        while (elapsedTime < fadeDuration)
        {
            float newAlpha = Mathf.Lerp(targetAlpha, 0f, elapsedTime / fadeDuration);
            DamageScreen.alpha = newAlpha;

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        damageScreenFade_Ref = null;
        DamageScreen.alpha = 0f;
    }

    IEnumerator DeathScreenFade(float targetAlpha, float fadeDuration)
    {
        float startAlpha = DeathScreen.alpha;
        float elapsedTime = 0.0f;
        ArmadilloPlayerController.Instance.inputControl.TogglePlayerControls(false);
        float timer = 0;
        while(timer<0.25f)
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
