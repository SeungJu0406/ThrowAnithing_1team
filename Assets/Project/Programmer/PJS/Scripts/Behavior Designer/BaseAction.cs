using BehaviorDesigner.Runtime.Tasks;

public class BaseAction : Action
{
    protected BaseEnemy baseEnemy;

    public override void OnAwake()
    {
        baseEnemy = GetComponent<BaseEnemy>();
    }
}