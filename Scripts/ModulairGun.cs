using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Modulair gun to mog Mairo's pathetic display of his gun system. 
/// This class serves as the core component of a modular gun system, 
/// allowing for dynamic addition and management of various modules that can enhance or modify the gun's behavior. 
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class ModulairGun : MonoBehaviour
{
    #region Variables

    [Header("Modules")]
    /// <summary>
    /// Serialized interface for the _modules array, allowing for the assignment of ScriptableObjects 
    /// that implement the IModule interface in the Unity Inspector.
    /// </summary>
    [SerializeField, InterfaceField(typeof(IModule), typeof(ScriptableObject))]
    private ScriptableObject[] _modules;

    private readonly List<IModule> _runtimeModules = new();

    /// <summary>
    /// List of all _modules on this modulair gun instance.
    /// These will be initialized on start and can subscribe to the shoot events to add their functionality to the gun.
    /// </summary>
    public IModule[] Modules
    {
        get
        {
            int serializedCount = _modules?.Length ?? 0;
            int runtimeCount = _runtimeModules.Count;
            if (serializedCount == 0 && runtimeCount == 0)
                return Array.Empty<IModule>();

            IModule[] modules = new IModule[serializedCount + runtimeCount];
            for (int i = 0; i < _modules.Length; i++)
                modules[i] = _modules[i] as IModule;

            for (int i = 0; i < _runtimeModules.Count; i++)
                modules[serializedCount + i] = _runtimeModules[i];

            return modules;
        }
    }

    [Header("Shoot Point")]
    [SerializeField]
    private Transform shootPoint;

    /// <summary>
    /// The end of the gun where the projectiles will be spawned from.
    /// </summary>
    public Transform ShootPoint => shootPoint;

    /// <summary>
    /// This event is called before the shoot action is executed. 
    /// Use this to prepare the gun for shooting, like playing a sound or animation.
    /// </summary>
    private event Action _preShoot;

    /// <summary>
    /// Represents the event that is raised when a shoot action occurs.
    /// </summary>
    private event Action _onShoot;

    /// <summary>
    /// This event is called after the shoot action is executed.
    /// Use this to clean up after shooting, like resetting the gun or playing a sound.
    /// </summary>
    private event Action _postShoot;

    #endregion

    #region Unity Lifetime
    private void Start()
    {
        if (_modules == null)
            return;

        foreach (var moduleObject in _modules)
        {
            if (moduleObject is IModule module)
                module.InitializeModule(this);
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Adds the given module to the _modules array and initializes it.
    /// </summary>
    /// <remarks>This method also iterates through the collection of _modules and invokes their <see cref="ModuleBase.UpdateModule"/> method,
    /// passing the current instance as a parameter. It is intended for internal use to ensure that all _modules remain
    /// synchronized with the state of this object.</remarks>
    public void AddModule(IModule mb)
    {
        if (mb == null)
            return;

        if (_modules == null)
            _modules = Array.Empty<ScriptableObject>();

        foreach (var module in Modules)
            module?.UpdateModule(this);

        if (mb is ScriptableObject moduleAsset)
        {
            Array.Resize(ref _modules, _modules.Length + 1);
            _modules[_modules.Length - 1] = moduleAsset;
        }
        else
        {
            _runtimeModules.Add(mb);
        }

        mb.InitializeModule(this);
    }

    public T CreateAndAddModule<T>() where T : ScriptableObject, IModule
    {
        T module = ScriptableObject.CreateInstance<T>();
        AddModule(module);
        return module;
    }

    /// <summary>
    /// Calls all shoot events.
    /// </summary>
    /// <param name="ctx"></param>
    public void Shoot(InputAction.CallbackContext ctx)
    {
        _preShoot?.Invoke();
        _onShoot?.Invoke();
        _postShoot?.Invoke();
    }

    #region Events
    /// <summary>
    /// Adds an action to the <see cref="_preShoot"/> event.
    /// </summary>
    /// <param name="action">The action to add to the event.</param>
    public void AddPreShootEvent(Action action) => _preShoot += action;

    /// <summary>
    /// Adds an action to the <see cref="_onShoot"/> event.
    /// </summary>
    /// <param name="action">The action to add to the event.</param>
    public void AddOnShootEvent(Action action) => _onShoot += action;

    /// <summary>
    /// Adds an action to the <see cref="_postShoot"/> event.
    /// </summary>
    /// <param name="action">The action to add to the event.</param>
    public void AddPostShootEvent(Action action) => _postShoot += action;

    #endregion

    #endregion
}
