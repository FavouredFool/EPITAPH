

public interface IState
{
    string GetKey();
    void OnEnter();
    void OnExit();
    void Update();
    void FixedUpdate();

}
