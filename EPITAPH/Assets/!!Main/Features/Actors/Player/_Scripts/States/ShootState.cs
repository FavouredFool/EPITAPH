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


        //float shootAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        //RaycastRotation = Quaternion.Euler(0f, 0f, angle);
        
        bolt.CacheRaycast(_ctx.PlayerController.Rb.position, _ctx.PlayerController.AimAssistedLookDirection);

        float shootStrength = PlayerVariableAnchor.PlayerVariables.Charge switch
        {
            1 => _ctx.PlayerController.ShootSpeedCharge1,
            2 => _ctx.PlayerController.ShootSpeedCharge2,
            _ => _ctx.PlayerController.ShootSpeedCharge3
        };
        
        float knockbackMultiplier = PlayerVariableAnchor.PlayerVariables.Charge switch
        {
            1 => 1,
            2 => _ctx.PlayerController.KnockbackMultiplier2,
            _ => _ctx.PlayerController.KnockbackMultiplier3
        };

        bolt.KnockbackMultiplier = knockbackMultiplier;

        bolt.GetShot(shootStrength);
        
        PlayerVariableAnchor.PlayerVariables.LoseAmmo(bolt);
        
        _ctx.PlayerController.Knockback(-_ctx.PlayerController.transform.up);

        // Play Audio
        PlayerAudio.PlayReleaseCrossbow();
        
        // Animation
        _ctx.PlayerController.CrossbowAnimator.SetTrigger(PlayerController.ShootCrossbowTriggerAnim);
        _ctx.PlayerController.CharacterAnimator.SetTrigger(PlayerController.ShotCharacterTriggerAnim);
        
        // Charge
        PlayerVariableAnchor.PlayerVariables.Charge = 0;
        PlayerVariableAnchor.PlayerVariables.ChargeProgress = 0;
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