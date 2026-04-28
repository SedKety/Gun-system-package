using UnityEngine;

[CreateAssetMenu(fileName = "Simple Bullet Factory", menuName = "Gun System/Factories/Simple Bullet Factory")]
public class SimpleBulletFactory : ScriptableObject, IProjectileFactory
{
    [SerializeField]
    private ProjectileFactorySettings _settings;

    public void SetUp(ProjectileFactorySettings settings)
    {
        _settings = settings;
    }

    public GameObject CreateBullet(Transform location)
    {
        if (_settings == null)
            return null;
        var bullet = Instantiate(_settings.projectilePrefab, location.position, location.rotation);
        return bullet;
    }

}
