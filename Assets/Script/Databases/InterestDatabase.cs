using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InterestDatabase", menuName = "Databases/InterestDatabase")]
public class InterestDatabase : ScriptableObject
{
    public List<InterestCategory> allInterests;
}
