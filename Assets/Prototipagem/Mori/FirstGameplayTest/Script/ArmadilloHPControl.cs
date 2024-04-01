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

    [Space, Header("Invulnerabiliy Time")]
    [System.NonSerialized] public bool isInvulnerable;
    public float invulnerabilityDuration;
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
}
