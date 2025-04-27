using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LevelProgression : MonoBehaviour
{
    [ReadOnly] 
    public List<Level> levels;

    [System.Serializable]
    public class Level
    {
        public List<IScriptableElement> unlockable = new List<IScriptableElement>();
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

    public void AddItemOnLevel(int level, IScriptableElement item)
    {
        if (level > 0)
        {
            level -= 1; // Convert to 0-based index
            if (level < 0 || level >= levels.Count)
            {
                for (int i = levels.Count; i <= level; i++)
                {
                    levels.Add(new Level());
                }
            }
            levels[level].unlockable.Add(item);
        }
    }
}
