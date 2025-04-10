using UnityEngine;

[CreateAssetMenu(fileName = "NewInterest", menuName = "Interest")]
public class InterestCategory : ScriptableObject
{
    #region "Variables"
    [field: SerializeField] public string interestName { get; private set; }
    [SerializeField] private string description;
    [field: SerializeField] public Sprite icon { get; private set; }
    #endregion
}
