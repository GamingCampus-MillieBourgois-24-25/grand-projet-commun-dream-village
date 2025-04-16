using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInhabitant", menuName = "ScriptableObjects/Character")]
public class Inhabitant : ScriptableObject
{

    #region Variables Serialized
    #region Basic Information
    [field: Header("Basic Information")]
    [field: SerializeField] public string FirstName { get; private set; }
    [field: SerializeField] public string LastName { get; private set; }
    [field: SerializeField] public Pronouns Pronouns { get; private set; }
    [field: SerializeField] public InhabitantEnums MBTI { get; private set; }
    [field: SerializeField] public List<Personalities> Personnality { get; private set; }
    #endregion

    #region Preferences
    [field: Header("Preferences")]
    [field: SerializeField] public List<InterestCategory> Likes { get; private set; }
    [field: SerializeField] public List<InterestCategory> Dislikes { get; private set; }
    #endregion

    #region Stats
    [field: Header("Statistics")]
    [field: SerializeField] public int Mood { get; private set; }
    [field: SerializeField] public int Serenity { get; private set; }
    [field: SerializeField] public int Energy { get; private set; }
    [field: SerializeField] public int Hearts { get; private set; }
    [field: SerializeField] public int Limit { get; private set; }
    #endregion

    #region Economy
    [field: Header("Progression & Economy")]
    [field: SerializeField] public float GoldMultiplier { get; private set; }
    [field: SerializeField] public int InitialPrice { get; private set; }
    [field: SerializeField] public bool CanLeave { get; private set; }
    [field: SerializeField] public int HeartsBeforeLeaving { get; private set; }
    #endregion

    #region Visuals
    [field: Header("Visuals")]
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public GameObject InhabitantPrefab { get; private set; }
    #endregion
    #endregion




    private void Awake()
    {
        Hearts = HeartsBeforeLeaving;
    }



    private int IsAffectedBy(InterestCategory element)
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
