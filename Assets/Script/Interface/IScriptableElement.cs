using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IScriptableElement : ScriptableObject
{
    #region Variables Serialized

    [field : Header("Base")]

    public virtual Player.InventoryCategory Category { get; set; }
    [field : SerializeField] public virtual string Name { get; private set; }
    [field: SerializeField] public virtual Sprite Icon { get; private set; }
    [field: SerializeField] public virtual int InitialPrice { get; private set; }
    [field: SerializeField] public virtual GameObject InstantiatePrefab { get; set; }

    #endregion
}