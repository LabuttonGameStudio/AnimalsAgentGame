using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage
{
    public Damage(float damageAmount, DamageType damageType,bool wasMadeByPlayer,Vector3 originPoint)
    {
        this.damageAmount = damageAmount;
        this.damageType = damageType;
        this.wasMadeByPlayer = wasMadeByPlayer;
        this.originPoint = originPoint;
    }

    public float damageAmount;

    public bool wasMadeByPlayer;

    public Vector3 originPoint;

    public DamageType damageType;
    public enum DamageType
    {
        Blunt,
        Slash,
        Eletric,
        Water,
        Hacking
    }
}
