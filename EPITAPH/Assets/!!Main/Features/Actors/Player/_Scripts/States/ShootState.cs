using UnityEngine;

public class ShootState : VampireBaseState
{
    public ShootState(VampireStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {
        BoltType type = _ctx.PlayerController.GetBoltTypeToShoot();
        
        BoltController bolt = Object.Instantiate(_ctx.PlayerController.ProjectileBlueprint, _ctx.PlayerController.transform.position + _ctx.PlayerController.transform.forward * _ctx.PlayerController.SpawnDist, _ctx.PlayerController.transform.rotation, _ctx.PlayerController.InstantiationParent);
        bolt.BoltType = type;
        bolt.Player = _ctx.PlayerController;
        
        _ctx.PlayerController.CurrentBoltsHeld[type] = bolt;
        _ctx.PlayerController.BoltInChamber = false;
        
        _ctx.PlayerController.Knockback(-_ctx.PlayerController.transform.up);

        // Play Audio
        PlayerAudio.PlayReleaseCrossbow();
        
        // Animation
        _ctx.PlayerController.CrossboxAnimator.SetTrigger(PlayerController.ShootCrossbowTriggerAnim);
        _ctx.PlayerController.CharacterAnimator.SetTrigger(PlayerController.ShotCharacterTriggerAnim);
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