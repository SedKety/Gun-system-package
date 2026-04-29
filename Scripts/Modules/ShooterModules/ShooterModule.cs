using UnityEngine;

/// <summary>
/// This module is responsible for shooting projectiles from the gun. 
/// </summary>
[CreateAssetMenu(fileName = "New Shooter Module", menuName = "Modulair Gun/Modules/Shooter Module")]
public class ShooterModule : ScriptableObject, IModule
{

    [SerializeField, InterfaceField(typeof(IProjectileFactory), typeof(ScriptableObject))]
    private ScriptableObject _projectileFactory;

    /// <summary>
    /// The projectile factory responsible for creating the bullets when the gun is shot.
    /// Changing the factory here will change the orientation and the type of bullet.
    /// </summary>
    public IProjectileFactory ProjectileFactory => _projectileFactory as IProjectileFactory;

    public void InitializeModule(ModularGun mg)
    {
        // When the gun is shot, we want to create a bullet at the shoot point.
        mg.AddOnShootEvent(() =>
        {
            var projectile = ProjectileFactory.CreateProjectile(mg.ShootPoint);
        });
    }

    public void UpdateModule(ModularGun mg)
    {
        
    }
}
