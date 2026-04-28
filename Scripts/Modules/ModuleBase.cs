using System;
using UnityEngine;

/// <summary>
/// The base class for all modules that can be added to a <see cref="ModulairGun"/>.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Provides an entry point for the module to initialize itself with the given ModulairGun instance. This method is called when the module is added to the gun, 
    /// allowing it to set up any necessary references or configurations.
    /// </summary>
    public abstract void InitializeModule(ModulairGun mg);

    /// <summary>
    /// Called upon a new module being added to this gun. 
    /// This method allows the module to perform any necessary updates or adjustments
    /// based on the current state of the gun and its <see cref="ModulairGun.Modules"/>.
    /// </summary>
    /// <param name="mg"></param>
    public abstract void UpdateModule(ModulairGun mg);
}
