using UnityEngine;

public class ShootState : VampireBaseState
{
    public ShootState(VampireStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {
        BoltType type = _ctx.PlayerController.GetBoltTypeToShoot();
        
        _ctx.PlayerController.CurrentBoltsHeld[type] = false;
        _ctx.PlayerController.BoltInChamber = false;
        
        BoltController bolt = Object.Instantiate(_projectileBlueprint, transform.position + transform.forward * _spawnDist, transform.rotation, _instantiationParent);
        bolt.BoltType = type;
        bolt.BloodpointPlayer = _bloodlineConnection;
        
        Knockback(-transform.up);

        // Play Audio
        PlayerAudio.PlayReleaseCrossbow();
        
        // Animation
        Debug.Log("SHOT");
        CrossboxAnimator.SetTrigger(PlayerController.ShootCrossbowTriggerAnim);
        CharacterAnimator.SetTrigger(PlayerController.ShotCharacterTriggerAnim);
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