using UnityEngine;
using System.Collections.Generic;



[CreateAssetMenu(fileName = "BuildingDatabase", menuName = "ScriptableObjects/Building", order = 1)]
public class Building : ScriptableObject
{
    #region Variable Serialized
    [Header("Building Information")]
    [SerializeField] public BuildingEffect effect;

    [Header("Stats")]
    [SerializeField] public int price;
    [SerializeField] int maximumInInv;
    [SerializeField] int unlockLevel;


    #endregion


    [System.Serializable]
    public class BuildingEffect
    {
        [SerializeField] public string name;
        [SerializeField] private string description;
        [SerializeField][Tooltip("in minutes")] private int utilisationDuration;

        [Header("Statistics")]
        [SerializeField] private int energy;
        [SerializeField] private int mood;
        [SerializeField] private int serenity;

        [Header("Stats")]
        [SerializeField] private List<AttributeEffect> attributeEffects;

        [Header("Visuals")]
        [SerializeField] public Sprite icon;
        [SerializeField] private GameObject buildingPrefab;


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