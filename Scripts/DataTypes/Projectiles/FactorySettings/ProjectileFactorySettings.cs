using UnityEngine;

/// <summary>
/// This class is responsible for storing all factory settings, 
/// like the projectile prefab to spawn when the factory is called to create a projectile.
/// </summary>
[CreateAssetMenu(fileName = "New Projectile Factory Settings", menuName = "Gun System/Projectiles/Factory Settings")]
public class ProjectileFactorySettings  : ScriptableObject
{
    /// <summary>
    /// The projectile to spawn when the factory is called to create a projectile.
    /// </summary>
    public GameObject projectilePrefab;
}
