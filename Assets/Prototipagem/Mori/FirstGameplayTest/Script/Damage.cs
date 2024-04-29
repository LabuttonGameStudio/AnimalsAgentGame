using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage
{
    public Damage(float damageAmount, DamageType damageType)
    {

    }

    public float damageAmount;
    public enum DamageType
    {
        Blunt,
        Eletric,
        Water,
        Slash
    }
}
