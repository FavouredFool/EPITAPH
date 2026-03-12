using UnityEngine;

public class ShootState : VampireBaseState
{
    public ShootState(VampireStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {
        BoltType type = _ctx.PlayerController.GetBoltTypeToShoot();
        
        BoltController bolt = Object.Instantiate(_ctx.PlayerController.ProjectileBlueprint, _ctx.PlayerController.transform.position + _ctx.PlayerController.transform.forward * _ctx.PlayerController.SpawnDist, _ctx.PlayerController.transform.rotation);
        bolt.BoltType = type;
        bolt.Player = _ctx.PlayerController;
        
        PlayerVariableAnchor.PlayerVariables.LoseAmmo(bolt);
        
        _ctx.PlayerController.Knockback(-_ctx.PlayerController.transform.up);

        // Play Audio
        PlayerAudio.PlayReleaseCrossbow();
        
        // Animation
        _ctx.PlayerController.CrossboxAnimator.SetTrigger(PlayerController.ShootCrossbowTriggerAnim);
        _ctx.PlayerController.CharacterAnimator.SetTrigger(PlayerController.ShotCharacterTriggerAnim);
        
        // Charge
        PlayerVariableAnchor.PlayerVariables.Charge = 0;
    }

    public override void Update()
    {
        
    }
    
    public override void FixedUpdate()
    {
        
    }

    public override void OnExit()
    {
        
    }
}