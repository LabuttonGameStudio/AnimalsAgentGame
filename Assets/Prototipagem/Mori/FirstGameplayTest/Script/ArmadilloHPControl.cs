using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmadilloHPControl : MonoBehaviour,IDamageable
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

    private bool isFadingIn = false;
    private bool isFadingOut = false;


    public void TakeDamage(int damageAmount)
    {
        if (!isInvulnerable)
        {
            if (currentHp - damageAmount < 0)
            {
                ArmadilloPlayerController.Instance.Die();
                StartCoroutine(DeathScreenFade(1f, 1f));
                currentHp = 0;
                UpdateHealthBar();
                return;
            }

            if (!isFadingIn && !isFadingOut)
            {
                StartCoroutine(DamageScreenFade(1f, 0.5f));
            }
            currentHp -= damageAmount;
            currentGreyHp = damageAmount / 2;
            UpdateHealthBar();
        }
    }
    public void OnHeal(float heal)
    {
        if (currentHp + currentGreyHp + heal > maxHp)
        {
            currentHp = maxHp;
            currentGreyHp = 0;
            UpdateHealthBar();
            return;
        }
        currentHp += heal + currentGreyHp;
        currentGreyHp = 0;
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


    private IEnumerator DamageScreenFade(float targetAlpha, float fadeDuration)
    {
        isFadingIn = true;

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

        // Espera um tempo antes de começar a diminuir o alpha
        yield return new WaitForSeconds(0.2f);

        isFadingIn = false;
        isFadingOut = true;

        elapsedTime = 0.0f;

        // Faz a tela de dano diminuir o alpha
        while (elapsedTime < fadeDuration)
        {
            float newAlpha = Mathf.Lerp(targetAlpha, 0f, elapsedTime / fadeDuration);
            DamageScreen.alpha = newAlpha;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        DamageScreen.alpha = 0f;

        isFadingOut = false;
    }

    IEnumerator DeathScreenFade(float targetAlpha, float fadeDuration)
    {
        float startAlpha = DeathScreen.alpha;
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            DeathScreen.alpha = newAlpha;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        DeathScreen.alpha = targetAlpha;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
}
