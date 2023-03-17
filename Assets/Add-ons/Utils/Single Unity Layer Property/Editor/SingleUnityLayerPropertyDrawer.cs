
using UnityEngine;
using UnityEditor;

namespace Utils.SingleUnityLayerProperty
{
    [CustomPropertyDrawer(typeof(SingleUnityLayer))]
    public class SingleUnityLayerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            SerializedProperty layerIndex = property.FindPropertyRelative("_layerIndex");

            if (layerIndex != null)
            {
                layerIndex.intValue = EditorGUI.LayerField(position, label, layerIndex.intValue);
            }
            
            EditorGUI.EndProperty();
        }
    }
}
