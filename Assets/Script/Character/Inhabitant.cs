using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInhabitant", menuName = "Character")]
public class Inhabitant : ScriptableObject
{

    #region Variables Serialized
    [field: Header("Basic Information")]
    [field: SerializeField] public string FirstName { get; private set; }
    [field: SerializeField] public string LastName { get; private set; }
    [field: SerializeField] public Pronouns Pronouns { get; private set; }
    [field: SerializeField] public InhabitantEnums MBTI { get; private set; }
    [field: SerializeField] public List<Personalities> Personnality { get; private set; }

    [field: Header("Preferences")]
    [field: SerializeField] public List<InterestCategory> Likes { get; private set; }
    [field: SerializeField] public List<InterestCategory> Dislikes { get; private set; }

    [field: Header("Statistics")]
    [field: SerializeField] public int Mood { get; private set; }
    [field: SerializeField] public int Serenity { get; private set; }
    [field: SerializeField] public int Energy { get; private set; }
    [field: SerializeField] public int Hearts { get; private set; }
    [field: SerializeField] public int Limit { get; private set; }

    [field: Header("Progression & Economy")]
    [field: SerializeField] public float GoldMultiplier { get; private set; }
    [field: SerializeField] public int UnlockLevel { get; private set; }
    [field: SerializeField] public int InitialPrice { get; private set; }
    [field: SerializeField] public bool CanLeave { get; private set; }
    [field: SerializeField] public int HeartsBeforeLeaving { get; private set; }

    [field: Header("Visuals")]
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public GameObject InhabitantPrefab { get; private set; }
    #endregion




    private void Awake()
    {
        Hearts = HeartsBeforeLeaving;
    }




    private void ModifyStatistic()
    {
        
    }

    private int isAffectedBy(InterestCategory element)
    {
        if (Likes.Contains(element))
        {
            return 1;
        }
        else if (Dislikes.Contains(element))
        {
            return -1;
        }

        return 0;
    }
}
