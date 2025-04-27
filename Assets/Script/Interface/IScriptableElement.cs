using UnityEngine;

public class IScriptableElement : ScriptableObject
{
    #region Variables Serialized

    [field : Header("Base")]

    public virtual Player.ItemCategory Category { get; set; }
    [field : SerializeField] public virtual string Name { get; private set; }
    [field: SerializeField] public virtual Sprite Icon { get; private set; }
    [field: SerializeField] public virtual int InitialPrice { get; private set; }
    [field: SerializeField] public virtual GameObject InstantiatePrefab { get; set; }
    [field: SerializeField] public virtual int MaxOwned { get; private set; }
    [field: SerializeField] public virtual int UnlockedAtLvl { get; private set; }

    #endregion
}