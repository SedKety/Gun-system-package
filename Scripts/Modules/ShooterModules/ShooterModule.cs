using UnityEngine;

[CreateAssetMenu(fileName = "New Shooter Module", menuName = "Modulair Gun/Modules/Shooter Module")]
public class ShooterModule : ScriptableObject, IModule
{
    [SerializeField, InterfaceField(typeof(IProjectileFactory), typeof(ScriptableObject))]
    private ScriptableObject _projectileFactory;

    public IProjectileFactory ProjectileFactory => _projectileFactory as IProjectileFactory;

    public void InitializeModule(ModulairGun mg)
    {
        mg.AddOnShootEvent(() =>
        {
            var projectile = ProjectileFactory.CreateBullet(mg.ShootPoint);
        });
    }

    public void UpdateModule(ModulairGun mg)
    {
        
    }
}
