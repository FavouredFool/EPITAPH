public abstract class EnemyBaseState : IState
{

    protected EnemyStateContext _ctx;
    protected EnemyBaseState(EnemyStateContext ctx)
    {
        _ctx = ctx;
    }
    
    public string GetKey()
    {
        return "";
    }

    public virtual void OnEnter()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void OnExit()
    {

    }
}