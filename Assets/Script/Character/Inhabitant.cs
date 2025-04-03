using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewInhabitant", menuName = "Character")]
public class Inhabitant : ScriptableObject
{

    #region Variables Serialized
    [Header("Basic Information")]
    [SerializeField] private string firstName;
    [SerializeField] private string lastName;
    [SerializeField] private Pronouns pronouns;
    [SerializeField] private InhabitantEnums MBTI;

    [SerializeField] private List<Personalities> personnality;

    [Header("Preferences")] 
    [SerializeField] private List<InterestCategory> likes;
    [SerializeField] private List<InterestCategory> dislikes;

    [Header("Statistics")] 
    private int mood;
    private int serenity;
    private int energy;
    private int hearts;
    [SerializeField] private int limit;

    [Header("Progression & Economy")] 
    [SerializeField] private float goldMultiplier;
    [SerializeField] private int unlockLevel;
    [SerializeField] private int InitialPrice;
    [SerializeField] private bool canLeave;
    [SerializeField] private int heartsBeforeLeaving;


    [Header("Visuals")]
    [SerializeField] private Image Icon;
    [SerializeField] private GameObject InhabitantPrefab;
    #endregion




    private void Awake()
    {
        hearts = heartsBeforeLeaving;
    }




    private void ModifyStatistic()
    {
        
    }

    private int isAffectedBy(InterestCategory element)
    {
        if (likes.Contains(element))
        {
            return 1;
        }
        else if (dislikes.Contains(element))
        {
            return -1;
        }

        return 0;
    }
}
