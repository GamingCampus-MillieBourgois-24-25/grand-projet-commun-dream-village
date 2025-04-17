using UnityEditor;
using UnityEngine;
using static LevelProgression;

[CustomPropertyDrawer(typeof(Level))]
public class LevelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty parentArray = property.serializedObject.FindProperty("levels");
        int index = GetIndex(property, parentArray);

        label.text = $"Level {index + 1}";
        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    private int GetIndex(SerializedProperty property, SerializedProperty parentArray)
    {
        for (int i = 0; i < parentArray.arraySize; i++)
        {
            if (SerializedProperty.EqualContents(parentArray.GetArrayElementAtIndex(i), property))
            {
                return i;
            }
        }
        return -1;
    }
}

