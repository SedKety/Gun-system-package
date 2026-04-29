using UnityEngine;

/// <summary>
/// Projectile factory responsible for shooting bullets in a straight line with no special behavior.
/// </summary>
[CreateAssetMenu(fileName = "Simple Bullet Factory", menuName = "Gun System/Factories/Simple Bullet Factory")]
public class SimpleBulletFactory : ScriptableObject, IProjectileFactory
{
    [SerializeField]
    private ProjectileFactorySettings _settings;

    public void Initialize(ProjectileFactorySettings settings)
    {
        _settings = settings;
    }

    public GameObject CreateProjectile(Transform location)
    {
        if (_settings == null)
            return null;
        var bullet = Instantiate(_settings.projectilePrefab, location.position, location.rotation);
        return bullet;
    }

}
