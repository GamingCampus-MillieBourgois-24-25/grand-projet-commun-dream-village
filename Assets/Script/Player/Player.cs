using UnityEngine;

public class Player
{
    public string PlayerName { get; private set; }
    public string CityName { get; private set; }

    public int Level { get; private set; } = 1;
    public int CurrentXP { get; private set; } = 0;

    [SerializeField] private int baseExpPerLevel = 300;
    [SerializeField] private float multExp = 1.3f;

    private int expLevel;

    private void Start()
    {
        expLevel = baseExpPerLevel;
    }
    public void AddXP(int amount)
    {
        CurrentXP += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if (CurrentXP >= expLevel) { 
            CurrentXP -= expLevel;
            Level++;
            expLevel = Mathf.RoundToInt(expLevel * multExp);
            Debug.Log($"Level Up! New level: {Level}");
        }
    }

    public void SetPlayerInfo(string name, string city)
    {
        PlayerName = name;
        CityName = city;
    }
}
