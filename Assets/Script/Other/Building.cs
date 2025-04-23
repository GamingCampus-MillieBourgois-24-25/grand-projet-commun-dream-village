using UnityEngine;
using System.Collections.Generic;
using static Dialogues;



[CreateAssetMenu(fileName = "BuildingDatabase", menuName = "ScriptableObjects/Building", order = 1)]
public class Building : IScriptableElement
{
    #region Variable Serialized
    #region Basic Information
    [field: Header("Basic Information")]
    public override Player.ItemCategory Category => Player.ItemCategory.BuildingCategory;
    [field: SerializeField] public override string Name => base.Name;
    [field: SerializeField] public string Description { get; private set; }
    #endregion

    #region Effects
    [field: Header("Effects")]
    [field: SerializeField] public int EffectDuration { get; private set; }
    [field: SerializeField] public List<AttributeEffect> AttributeEffects { get; private set; }
    [field: SerializeField] public int Experience { get; private set; }
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
    [field: SerializeField] public override int MaxOwned => base.MaxOwned;
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
        public enum BonusType
        {
            Multiple,
            Add
        }


        [field: SerializeField] public InterestCategory attribute { get; private set; }
        [field: SerializeField] public float bonus { get; private set; }
        [field: SerializeField] public BonusType bonusType { get; private set; }
    }
}