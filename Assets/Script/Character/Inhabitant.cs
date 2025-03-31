using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInhabitant", menuName = "Character")]
public class Inhabitant : ScriptableObject
{
    [Header("Basic Information")]
    public string firstName;
    public string lastName;
    public string pronouns;
    public string MBTI;

    [TextArea] public string personnality;

    [Header("Preferences")] 
    public List<InterestCategory> likes;
    public List<InterestCategory> dislikes;

    [Header("Statistics")] 
    public int mood;
    public int serenity;
    public int energy;
    public int hearts;

    [Header("Progression & Economy")] 
    public float goldMultiplier;
    public int unlockLevel;
    public int InitialPrice;
    public bool canLeave;

    public void ModifyStatistic()
    {
        
    }

    public bool isAffectedBy(InterestCategory element)
    {
        if (likes.Contains(element))
        {
            return true;
        }
        else if (dislikes.Contains(element))
        {
            return false;
        }

        return true;
    }
}
