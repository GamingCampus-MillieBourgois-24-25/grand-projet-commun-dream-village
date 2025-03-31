using UnityEngine;
using System.Collections.Generic;


public enum BuildingType
{
    Bench,
    Puit,
    Fountain,
    Cafe,
    Theater,
    Gym,
    SwimmingPool,
    Library
}

[System.Serializable]
public class BuildingEffect
{
    public string effectName;
    public int effectValue;
}

[System.Serializable]
public class BuildingData
{
    public BuildingType name;
    public int cost;
    public string usageTime;
    public List<BuildingEffect> Effects;
    public int maxAmount;
}

[CreateAssetMenu(fileName = "BuildingDatabase", menuName = "ScriptableObjects/BuildingDatabase", order = 1)]
public class Utiles : ScriptableObject
{
    public BuildingData buildings;
    
    public void PrintBuildingData()
    {
        Debug.Log("Building Name: " + buildings.name);
        Debug.Log("Cost: " + buildings.cost);
        Debug.Log("Usage Time: " + buildings.usageTime);
        foreach (var effect in buildings.Effects)
        {
            Debug.Log("Effect Name: " + effect.effectName + ", Effect Value: " + effect.effectValue);
        }
        Debug.Log("Max Amount: " + buildings.maxAmount);
    }
}