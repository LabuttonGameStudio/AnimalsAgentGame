using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : IEnemy, IDamageable, ISoundReceiver
{
    //-----Variables-----
    #region State Machine Variables
    protected RangedEnemy currentState;

    protected MeleeEnemyRoamingState enemyRoamingState;
    protected MeleeEnemyObservingState enemyObservingState;
    protected MeleeEnemySearchingState enemySearchingState;
    protected MeleeEnemyAttackingState enemyAttackingState;
    #endregion

    //-----Action Update-----
    #region Action Update Functions
    public override void OnActionUpdate()
    {

    }
    #endregion

    //-----Hear-----
    #region Hear Functions
    public void OnSoundHear(SoundData soundData)
    {

    }
    #endregion

    //-----Visibility Update-----
    #region Visibility Update Functions
    public override void OnVisibilityUpdate()
    {

    }
    #endregion

    //-----Take Damage-----
    #region Take Damage Functions
    public void TakeDamage(Damage damage)
    {

    }
    #endregion

    //-----Base Functions-----
    #region Base Functions
    protected override void OnAwake()
    {

    }
    protected override void OnStart()
    {

    }
    protected override void OnFixedUpdate()
    {

    }
    #endregion

    //-----Pathfinding-----
    #region Pathfinding Functions
    protected override void OnRoamingPathEnd()
    {

    }
    #endregion
}
