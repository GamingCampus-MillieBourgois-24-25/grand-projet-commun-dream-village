using System.Collections.Generic;
using UnityEngine;

public class InhabitantInstance : ISaveable<InhabitantInstance.SavePartData>
{
    public Inhabitant baseData;
    private int mood;
    private int serenity;
    private int energy;
    private int hearts;

    public HashSet<InterestCategory> DiscoveredLikes = new();
    public HashSet<InterestCategory> DiscoveredDislikes = new();

    public int Mood
    {
        get => mood;
        set => mood = Mathf.Clamp(value, -100, 100);
    }

    public int Serenity
    {
        get => serenity;
        set => serenity = Mathf.Clamp(value, -100, 100);
    }

    public int Energy
    {
        get => energy;
        set => energy = Mathf.Clamp(value, -100, 100);
    }

    public int Hearts
    {
        get => hearts;
        set => hearts = Mathf.Clamp(value, 0, baseData.Limit);
    }

    public InhabitantInstance(Inhabitant data)
    {
        baseData = data;
        Mood = data.Mood;
        Serenity = data.Serenity;
        Energy = data.Energy;
        Hearts = data.Hearts;
    }

    public InhabitantInstance() { }
    
    public void DiscoverInterest(InterestCategory category)
    {
        if (baseData.Likes.Contains(category))
            DiscoveredLikes.Add(category);

        if (baseData.Dislikes.Contains(category))
            DiscoveredDislikes.Add(category);
    }



    public string Name => baseData.Name;
    public Sprite Icon => baseData.Icon;
    public List<InterestCategory> Likes => baseData.Likes;
    public List<InterestCategory> Dislikes => baseData.Dislikes;

    public float GoldMultiplier => baseData.GoldMultiplier;



    public void FinishActivity(List<Building.AttributeEffect> _attributes, int _energy, int _mood, int _serenity)
    {
        foreach (var attribute in _attributes)
        {
            InterestCategory category = attribute.attribute;
            float bonus = attribute.bonus;

            int isAffected = baseData.IsAffectedBy(category);

            if (isAffected != 0)
            {
                switch (attribute.bonusType)
                {
                    case Building.AttributeEffect.BonusType.Add:
                        Mood += Mathf.RoundToInt(_mood + bonus * isAffected);
                        Serenity += Mathf.RoundToInt(_serenity + bonus * isAffected);
                        Energy += Mathf.RoundToInt(_energy + bonus * isAffected);
                        break;

                    case Building.AttributeEffect.BonusType.Multiple:
                        Mood += Mathf.RoundToInt(_mood * (bonus * isAffected));
                        Serenity += Mathf.RoundToInt(_serenity * (bonus * isAffected));
                        Energy += Mathf.RoundToInt(_energy * (bonus * isAffected));
                        break;
                }
            }
            else
            {
                Mood += _mood;
                Serenity += _serenity;
                Energy += _energy;
            }
        }
    }







    public class SavePartData : ISaveData
    {
        public string baseInhabitantName;

        public int mood;
        public int serenity;
        public int energy;
        public int hearts;
        public HashSet<InterestCategory> discoveredLikes = new();
        public HashSet<InterestCategory> discoveredDislikes = new();
    }

    public SavePartData Serialize()
    {
        var data = new SavePartData();

        data.baseInhabitantName = baseData.Name;

        data.mood = Mood;
        data.serenity = Serenity;
        data.energy = energy;
        data.hearts = Hearts;

        data.discoveredLikes = DiscoveredLikes;
        data.discoveredDislikes = DiscoveredDislikes;

        return data;
    }

    public void Deserialize(SavePartData data)
    {
        baseData = GM.Instance.GetInhabitantByName(data.baseInhabitantName);
        Mood = data.mood;
        Serenity = data.serenity;
        Energy = data.energy;
        Hearts = data.hearts;
        DiscoveredLikes = data.discoveredLikes;
        DiscoveredDislikes = data.discoveredDislikes;
    }
}
