using UnityEngine;

/// <summary>
/// This is the basic cooldown module, it prevents shooting for a certain amount of time after each shot.
/// </summary>
[CreateAssetMenu(fileName = "CooldownModule", menuName = "Gun System/Modules/Cooldown Module")]
public class CooldownModule : ScriptableObject, IGunModule
{
    /// <summary>
    /// The time in seconds that the gun will be on cooldown after each shot. During this time, the gun cannot shoot.
    /// </summary>
    [SerializeField] private float _cooldownTime = 1f;

    /// <summary>
    /// The time since the last shot was fired. 
    /// This is used to track the cooldown time and determine if the gun can shoot again.
    /// </summary>
    private float _timeSinceLastShot = 0f;

    /// <summary>
    /// The modular gun to which this module is attached.
    /// </summary>
    private ModularGun _parentGun;

    public void InitializeModule(ModularGun mg) 
    { 
        //Update
        mg.SubscribeToModuleUpdateEvent(OnUpdate);

        //Pre-Shoot
        mg.SubscribeToPreShootEvent(OnPreShoot);

        //Shoot
        mg.SubscribeToOnShootEvent(OnShoot);

    }

    public void RecomputeModule(ModularGun mg) { }

    /// <summary>
    /// This method hooks into the <see cref="ModularGun.SubscribeToModuleUpdateEvent(System.Action)"/>  event to track the cooldown time.
    /// </summary>
    private void OnUpdate() => _timeSinceLastShot += Time.deltaTime;

    /// <summary>
    /// Checks whether the gun is currently on cooldown by comparing the time since the last shot with the cooldown time. 
    /// If the gun is on cooldown, it will not be able to shoot.
    /// </summary>
    /// <returns>Whether the gun is on cooldown or not.</returns>
    private bool IsOnCooldown() => _timeSinceLastShot < _cooldownTime;

    /// <summary>
    /// This method is called upon shooting and resets the cooldown timer by setting the time since the last shot to zero.
    /// </summary>
    private void OnShoot() => _timeSinceLastShot = 0f;

    /// <summary>
    /// Called before the shoot event happens, toggling shooting is set here.
    /// </summary>
    private void OnPreShoot()
    {
        if(IsOnCooldown() && _parentGun.CanShoot)
            _parentGun.ToggleCanShoot();
    }
}
