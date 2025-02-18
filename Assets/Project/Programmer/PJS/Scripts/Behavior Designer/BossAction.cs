
public class BossAction : BaseAction
{
	protected BossEnemy bossEnemy;

    public override void OnAwake()
    {
        bossEnemy = GetComponent<BossEnemy>();
    }
}