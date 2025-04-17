using System.Collections.Generic;
using UnityEngine;

public class LevelProgression : MonoBehaviour
{
    [SerializeField] List<Level> levels;

    [System.Serializable]
    public class Level
    {
        public List<ScriptableObject> unlockable;
    }

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
