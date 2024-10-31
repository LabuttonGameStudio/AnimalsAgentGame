public abstract class BombardierEnemyState
{
    public BombardierEnemyState(EnemyBombardier enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }
    public EnemyBombardier iEnemy;
    public abstract void OnVisibilityUpdate();
    public abstract void OnActionUpdate();
    public abstract void OnFixedUpdate();
    public abstract void OnEnterState();
    public abstract void OnExitState();
}
