using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelProgression : MonoBehaviour
{
    [SerializeField] List<Level> levels;

    [System.Serializable]
    public class Level
    {
        [SerializeField] private List<ScriptableObject> unlockable;
    }

    #region properties drawer to change name of the list
    [CustomPropertyDrawer(typeof(Level))]
    class LevelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Récupération de l'index du niveau
            SerializedProperty levelNameProp = property.FindPropertyRelative("levelName");
            SerializedProperty parentArray = property.serializedObject.FindProperty("levels");
            int index = GetIndex(property, parentArray);

            // Génération du nom dynamique
            label.text = $"Level {index + 1}";

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        // Méthode pour obtenir l'index de l'élément dans la liste
        private int GetIndex(SerializedProperty property, SerializedProperty parentArray)
        {
            for (int i = 0; i < parentArray.arraySize; i++)
            {
                if (SerializedProperty.EqualContents(parentArray.GetArrayElementAtIndex(i), property))
                {
                    return i;
                }
            }
            return -1; // Si non trouvé
        }
    }
    #endregion

    public Level GetLevel(int level)
    {
        level -= 1; // Convert to 0-based index
        if (level < 0 || level >= levels.Count)
        {
            Debug.LogError("Level out of range");
            return null;
        }
        return levels[level];
    }
}
