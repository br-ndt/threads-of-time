using UnityEngine;
using System;

/// <summary>
/// Restricts a ScriptableObject or MonoBehaviour field to specific subtypes.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class AllowedTypesAttribute : PropertyAttribute
{
    public Type[] Types { get; }

    public AllowedTypesAttribute(params Type[] types)
    {
        this.Types = types;
    }
}