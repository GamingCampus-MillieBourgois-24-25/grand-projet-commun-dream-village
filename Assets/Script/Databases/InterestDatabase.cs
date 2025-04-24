using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InterestDatabase", menuName = "Databases/InterestDatabase")]
public class InterestDatabase : ScriptableObject
{
    public List<InterestCategory> allInterests;

    public InterestCategory GetInterestByName(string name)
    {
        foreach (InterestCategory interest in allInterests)
        {
            if (interest.interestName == name)
            {
                return interest;
            }
        }
        Debug.LogError("Interest not found: " + name);
        return null;
    }
}
