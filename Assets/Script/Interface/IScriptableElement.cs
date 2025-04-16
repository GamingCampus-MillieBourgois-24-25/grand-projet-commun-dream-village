using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IScriptableElement : ScriptableObject
{
    #region Variables Serialized

    public virtual string Name { get; private set; }
    public virtual Sprite Icon { get; private set; }
    public virtual int InitialPrice { get; private set; }

    #endregion
}