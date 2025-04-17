using UnityEngine;
using System.Collections.Generic;
using static Dialogues;



[CreateAssetMenu(fileName = "BuildingDatabase", menuName = "ScriptableObjects/Building", order = 1)]
public class Building : IScriptableElement
{
    #region Variable Serialized
    #region Basic Information
    [field: Header("Basic Information")]
    [field: SerializeField] public override string Name => base.Name;
    [field: SerializeField] public string Description { get; private set; }
    #endregion

    #region Effects
    [field: Header("Effects")]
    [field: SerializeField] public int EffectDuration { get; private set; }
    [field: SerializeField] public List<AttributeEffect> AttributeEffects { get; private set; }
    #endregion

    #region Stats
    [field: Header("Stats")]
    [field: SerializeField] public int Mood { get; private set; }
    [field: SerializeField] public int Serenity { get; private set; }
    [field: SerializeField] public int Energy { get; private set; }
    #endregion

    #region Economy
    [field: Header("Progression & Economy")]
    [field: SerializeField] public override int InitialPrice => base.InitialPrice;
    [field: SerializeField] public int MaximumInInv { get; private set; }
    #endregion

    #region Visuals
    [field: Header("Visuals")]
    public override Sprite Icon => base.Icon;
    [field: SerializeField] public override GameObject InstantiatePrefab => base.InstantiatePrefab;
    #endregion
    #endregion


    [System.Serializable]
    public class AttributeEffect
    {
        enum BonusType
        {
            Multiple,
            Add
        }


        [field: SerializeField] private InterestCategory attribute;
        [field: SerializeField] private float bonus;
        [field: SerializeField] private BonusType bonusType;
    }
}