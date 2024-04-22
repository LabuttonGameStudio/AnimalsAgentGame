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

    [Space, Header("Invulnerabiliy Time")]
    [System.NonSerialized] public bool isInvulnerable;
    public float invulnerabilityDuration;

    private float initialAlpha = 0f;
    private float screenAlpha = 0.5f;
    public void TakeDamage(int damageAmount)
    {
        if (!isInvulnerable)
        {
            if (currentHp - damageAmount < 0)
            {
                ArmadilloPlayerController.Instance.Die();
                currentHp = 0;
                UpdateHealthBar();
                return;
            }
            currentHp -= damageAmount;
            currentGreyHp = damageAmount / 2;
            StartCoroutine(DamageScreenAlpha(1f, 0.2f));
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

   
    private IEnumerator DamageScreenAlpha(float screenAlpha, float time)
    {
        float startScreenAlpha = DamageScreen.alpha;
        float elapsedTime = 0.0f;
       

        while (elapsedTime < time)
        {
           
            float newAlpha = Mathf.Lerp(startScreenAlpha, screenAlpha, elapsedTime / time);
            DamageScreen.alpha = newAlpha;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        DamageScreen.alpha = screenAlpha;


        yield return new WaitForSeconds(0.2f);

        StartCoroutine(DamageScreenAlpha(initialAlpha, 0.2f));
    }
}
