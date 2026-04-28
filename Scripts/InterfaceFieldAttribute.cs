using System;
using UnityEngine;

public class InterfaceFieldAttribute : PropertyAttribute
{
    public Type InterfaceType;
    public Type SearchType;

    public InterfaceFieldAttribute(Type interfaceType)
    {
        InterfaceType = interfaceType;
        SearchType = typeof(UnityEngine.Object);
    }

    public InterfaceFieldAttribute(Type interfaceType, Type searchType)
    {
        if (searchType == null)
            throw new ArgumentNullException(nameof(searchType));

        if (!typeof(UnityEngine.Object).IsAssignableFrom(searchType))
            throw new ArgumentException("Search type must inherit from UnityEngine.Object", nameof(searchType));

        InterfaceType = interfaceType;
        SearchType = searchType;
    }
}