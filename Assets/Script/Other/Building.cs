using UnityEngine;
using System.Collections.Generic;
using static Dialogues;



[CreateAssetMenu(fileName = "BuildingDatabase", menuName = "ScriptableObjects/Building", order = 1)]
public class Building : ScriptableObject
{
    #region Variable Serialized
    #region Basic Information
    [field: Header("Basic Information")]
    [field: SerializeField] public string Name { get; private set; }
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
    [field: SerializeField] public int InitialPrice { get; private set; }
    [field: SerializeField] public int MaximumInInv { get; private set; }
    #endregion

    #region Visuals
    [field: Header("Visuals")]
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public GameObject BuildingPrefab { get; private set; }
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