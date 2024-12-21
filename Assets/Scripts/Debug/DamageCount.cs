using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCount : MonoBehaviour
{
    [SerializeField] private TextMesh textMesh;

    [SerializeField]private DamageableHitbox[] connectedHitboxes;
    private void Start()
    {
        foreach(DamageableHitbox hitbox in connectedHitboxes)
        {
            hitbox.OnTakeDamage.AddListener(OnTakeDamage);
        }
    }
    public void OnTakeDamage(Damage damage)
    {
        textMesh.text = damage.damageAmount.ToString();
    }
}
