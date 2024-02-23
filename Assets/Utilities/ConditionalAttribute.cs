using System;
using UnityEngine;

namespace Utilities {

/// <summary>
///     Attribute to decide whether or not to draw a property inside the editor.
///     It takes the name of the field to compare the value with and the value which is allowed for the property to be displayed.
///     WIP: at the moment it takes a string and the comparison is available only for strings or enum types.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public sealed class ConditionalAttribute : PropertyAttribute {
    public ConditionalAttribute(string comparedAttributeName, string[] validValues) {
        ComparedAttributeName = comparedAttributeName;
        ComparedValue = validValues;
    }

    public ConditionalAttribute(string comparedAttributeName, bool validValue) {
        ComparedAttributeName = comparedAttributeName;
        ComparedValue = new[] {validValue.ToString()};
    }

    public string[] ComparedValue { get; }

    public string ComparedAttributeName { get; }
}

}
