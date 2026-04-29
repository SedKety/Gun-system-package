using UnityEngine;

/// <summary>
/// Factory responsible for creating projectiles when needed.
/// E.g: When a gun is shot, when a grenade with shrapnell explodes or when a rocket hits a wall and spawns an explosion.
/// </summary>
public interface IProjectileFactory
{
    /// <summary>
    /// Allows for initializing the factory, such as pre-calculating math or caching references to objects. 
    /// This is called when the factory is created, so it can be used to prepare the factory for use.
    /// </summary>
    /// <param name="settings">The settings to initialize the factory with.</param>
    public abstract void Initialize(ProjectileFactorySettings settings);

    /// <summary>
    /// This method produces and initializes a projectile at the given origin.
    /// </summary>
    /// <param name="origin">The origin where the projectile should be created.</param>
    /// <returns>The created projectile GameObject.</returns>
    public abstract GameObject CreateProjectile(Transform origin);
}
