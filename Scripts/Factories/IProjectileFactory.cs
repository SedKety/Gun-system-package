using UnityEngine;

public interface IProjectileFactory
{
    public abstract void SetUp(ProjectileFactorySettings settings);
    public abstract GameObject CreateBullet(Transform location);
}
