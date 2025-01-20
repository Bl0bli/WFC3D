using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace WFC3D.Editor
{
    [CustomPropertyDrawer(typeof(VisibleInDebugInspector))]
    public class VisibleInDebugInspectorDrawer : PropertyDrawer
    {
       public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
       {
           return EditorGUI.GetPropertyHeight(property,true);
       }
       
       public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
       { 
           EditorGUI.PropertyField(position, property, label, true);
       }
    }
}
