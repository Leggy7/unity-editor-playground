using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace Utilities {

[CustomPropertyDrawer(typeof(ConditionalAttribute))]
public class ConditionalPropertyDrawer : PropertyDrawer {
    // Field that is being compared.
    private SerializedProperty comparedField;

    // reference to the attribute to be drawn conditionally
    private ConditionalAttribute drawIf;

    // Height of the property.
    private float propertyHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return propertyHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        drawIf = attribute as ConditionalAttribute;
        comparedField = property.serializedObject.FindProperty(drawIf!.ComparedAttributeName);

        var match = false;
        foreach (var value in drawIf.ComparedValue) {
            switch (comparedField.propertyType) {
                case SerializedPropertyType.Enum:
                    var enumValues = comparedField.enumNames;
                    var enumIndex = comparedField.enumValueIndex;
                    var selectedEnvironmentAsString = enumValues[enumIndex].ToLower(CultureInfo.CurrentCulture);
                    if (string.Compare(
                            selectedEnvironmentAsString, value.ToLower(CultureInfo.CurrentCulture), StringComparison.Ordinal
                        ) == 0) {
                        match = true;
                    }

                    break;
                case SerializedPropertyType.String:
                    if (string.Compare(
                            comparedField.stringValue, value.ToLower(CultureInfo.CurrentCulture), StringComparison.Ordinal
                        ) == 0) {
                        match = true;
                    }

                    break;
                case SerializedPropertyType.Boolean:
                    match |= bool.Parse(drawIf.ComparedValue[0]) == comparedField.boolValue;
                    break;
                default:
                    throw new ArgumentException($"Unhandled {comparedField.propertyType.ToString()} attribute type.");
            }
        }
        // get the value to be compared based on attribute type

        if (match) { // if condition is matched we give an height to the element and draw it
            propertyHeight = base.GetPropertyHeight(property, label);
            EditorGUI.PropertyField(position, property);
        } else {
            propertyHeight = 0;
        }
    }
}

}
