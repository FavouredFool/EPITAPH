
public abstract class VampireBaseState : IState
{

    protected VampireStateContext _ctx;
    protected VampireBaseState(VampireStateContext ctx)
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