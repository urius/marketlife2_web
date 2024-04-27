using Other;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(LabeledArrayAttribute))]
    public class LabeledArrayDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            try
            {
                var fieldName = ((LabeledArrayAttribute)attribute).FieldName;
                var field = property.boxedValue.GetType().GetField(fieldName);
                var fieldValue = field.GetValue(property.boxedValue);
                EditorGUI.PropertyField(rect, property, new GUIContent(fieldValue.ToString()), true);
            }
            catch
            {
                EditorGUI.PropertyField(rect, property, label, true);
            }
            EditorGUI.EndProperty();
        }
    }
}