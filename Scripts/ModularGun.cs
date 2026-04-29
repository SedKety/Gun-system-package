using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// This script serves the following purpose:
// To teach a friend of mine how inheritance, abstraction and composition
// can be used together to create a flexible and extensible gun system.


/// <summary>
/// Gun that can be modified by adding or removing modules that implement the <see cref="IGunModule"/> interface.
/// Modules can be added in the inspector as ScriptableObjects or at runtime as regular classes.
/// Modules can subscribe to the shoot events to add their functionality to the gun, such as shooting projectiles, playing sounds or animations, applying cooldowns, etc.
/// </summary>
/// TODO: Seperate input from the gun, allowing for more flexible input handling and the possibility to use the gun without input (e.g., for turrets or AI).
/// TODO: Add events for when the gun is equipped or unequipped, allowing modules to perform setup or cleanup when the gun is used or not used.
/// TODO: Add a method to remove modules from the gun, allowing for more dynamic gameplay and the possibility to change the gun's functionality during runtime.
[RequireComponent(typeof(PlayerInput))]
public class ModularGun : MonoBehaviour
{
    #region Variables

    [Header("Modules")]
    /// <summary>
    /// Serialized interface for the _modules array, allowing for the assignment of ScriptableObjects 
    /// that implement the IGunModule interface in the Unity Inspector.
    /// </summary>
    [SerializeField, InterfaceField(typeof(IGunModule), typeof(ScriptableObject))]
    private ScriptableObject[] _modules;

    private readonly List<IGunModule> _runtimeModules = new();

    /// <summary>
    /// List of all _modules on this modulair gun instance.
    /// These will be initialized on start and can subscribe to the shoot events to add their functionality to the gun.
    /// </summary>
    public IGunModule[] Modules
    {
        get
        {
            int serializedCount = _modules?.Length ?? 0;
            int runtimeCount = _runtimeModules.Count;
            if (serializedCount == 0 && runtimeCount == 0)
                return Array.Empty<IGunModule>();

            IGunModule[] modules = new IGunModule[serializedCount + runtimeCount];
            for (int i = 0; i < _modules.Length; i++)
                modules[i] = _modules[i] as IGunModule;

            for (int i = 0; i < _runtimeModules.Count; i++)
                modules[serializedCount + i] = _runtimeModules[i];

            return modules;
        }
    }

    /// <summary>
    /// Provides an interface for modules to subscribe to the update method without needing to inherit from Monobehavior.
    /// </summary>
    private event Action _moduleUpdateEvent;

    [Header("Shoot Point")]
    [SerializeField]
    private Transform shootPoint;

    /// <summary>
    /// The end of the gun where the projectiles will be spawned from.
    /// </summary>
    public Transform ShootPoint => shootPoint;


    [Header("Shooting settings")]
    /// <summary>
    /// Decides whether or not the <see cref="_onShoot"/> is invoked.
    /// </summary>
    public bool CanShoot => _canShoot;

    private bool _canShoot = true;

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

    /// <summary>
    /// This event is called when the shoot action is attempted but the gun is not allowed to shoot (i.e., when <see cref="_canShoot"/> is false).
    /// </summary>
    private event Action _onShootFailed;

    #endregion

    #region Unity Lifetime
    private void Start() => Initialize();

    private void Update() => _moduleUpdateEvent?.Invoke();
    #endregion

    #region Methods

    /// <summary>
    /// Initializes all the modules on the gun upon starting the game. 
    /// </summary>
    private void Initialize()
    {
        if (_modules == null)
            return;

        //Initialize all modules on start, allowing them to subscribe to the shoot events and perform any necessary setup.
        foreach (var moduleObject in _modules)
        {
            if (moduleObject is IGunModule module)
                module.InitializeModule(this);
        }
    }
    /// <summary>
    /// Adds the given module to the _modules array and initializes it.
    /// </summary>
    /// <remarks>This method also iterates through the collection of _modules and invokes their <see cref="ModuleBase.UpdateModule"/> method,
    /// passing the current instance as a parameter. It is intended for internal use to ensure that all _modules remain
    /// synchronized with the state of this object.</remarks>
    public void AddModule(IGunModule mb)
    {
        if (mb == null)
            return;

        if (_modules == null)
            _modules = Array.Empty<ScriptableObject>();

        foreach (var module in Modules)
            module?.RecomputeModule(this);

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

    /// <summary>
    /// Creates and adds a module to the Module array, this passes through <see cref="AddModule(IGunModule)"/>.
    /// </summary>
    /// <typeparam name="T">The type of the module to create and add.</typeparam>
    /// <returns>The created module.</returns>
    public T CreateAndAddModule<T>() where T : ScriptableObject, IGunModule
    {
        T module = ScriptableObject.CreateInstance<T>();
        AddModule(module);
        return module;
    }

    /// <summary>
    /// Calls all shoot events in order.
    /// </summary>
    /// <param name="ctx"></param>
    public void Shoot(InputAction.CallbackContext ctx)
    {
        //Call the pre-shoot event to prepare the gun for shooting.
        _preShoot?.Invoke();

        //Check if the gun can shoot. If not, invoke the shoot failed event and return.
        if (!_canShoot)
        {
            _onShootFailed?.Invoke();
            return;
        }

        //Call the on-shoot event to execute the shoot action.
        _onShoot?.Invoke();

        //Clean up in the post-shoot event.
        _postShoot?.Invoke();
    }


    public void ToggleCanShoot() => _canShoot = !_canShoot;

    #region Events
    /// <summary>
    /// Adds an action to the <see cref="_preShoot"/> event.
    /// </summary>
    /// <param name="action">The action to add to the event.</param>
    public void SubscribeToPreShootEvent(Action action) => _preShoot += action;

    /// <summary>
    /// Adds an action to the <see cref="_onShoot"/> event.
    /// </summary>
    /// <param name="action">The action to add to the event.</param>
    public void SubscribeToOnShootEvent(Action action) => _onShoot += action;

    /// <summary>
    /// Adds an action to the <see cref="_postShoot"/> event.
    /// </summary>
    /// <param name="action">The action to add to the event.</param>
    public void SubscribeToPostShootEvent(Action action) => _postShoot += action;

    /// <summary>
    /// Adds an action to the <see cref="_moduleUpdateEvent"/> event.
    /// This allows modules to subscribe to the update method without needing to inherit from Monobehavior.
    /// </summary>
    /// <param name="action">The action to add to the event.</param>
    public void SubscribeToModuleUpdateEvent(Action action) => _moduleUpdateEvent += action;

    #endregion

    #endregion
}
