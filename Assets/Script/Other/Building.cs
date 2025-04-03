using UnityEngine;
using System.Collections.Generic;



[CreateAssetMenu(fileName = "BuildingDatabase", menuName = "Building", order = 1)]
public class Building : ScriptableObject
{
    #region Variable Serialized
    [Header("Building Information")]
    [SerializeField] private BuildingEffect effect;

    [Header("Stats")]
    [SerializeField] int price;
    [SerializeField] int maximumInInv;
    [SerializeField] int unlockLevel;


    #endregion


    [System.Serializable]
    class BuildingEffect
    {
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField][Tooltip("in minutes")] private int utilisationDuration;

        [Header("Statistics")]
        [SerializeField] private int energy;
        [SerializeField] private int mood;
        [SerializeField] private int serenity;

        [Header("Stats")]
        [SerializeField] private List<AttributeEffect> attributeEffects;


        [System.Serializable]
        class AttributeEffect
        { 
            enum BonusType
            {
                Multiple,
                Add
            }


            [SerializeField] private InterestCategory attribute;
            [SerializeField] private float bonus;
            [SerializeField] private BonusType bonusType;
        }
    }

    
}