using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewInterest", menuName = "Interest")]
public class InterestCategory : ScriptableObject
{
    #region "Variables"
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private Image icon;
    #endregion
}
