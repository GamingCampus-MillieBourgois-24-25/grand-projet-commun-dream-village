using System.Collections.Generic;
using UnityEngine;

public class InhabitantInstance : ISaveable<InhabitantInstance.SavePartData>
{
    public Inhabitant baseData;
    public HouseObject houseObject;
    public bool isInActivity = false;

    private int mood;
    private int serenity;
    private int energy;
    private int hearts;

    public HashSet<InterestCategory> DiscoveredLikes = new();
    public HashSet<InterestCategory> DiscoveredDislikes = new();

    public int Mood
    {
        get => mood;
        set => mood = Mathf.Clamp(value, -baseData.Limit, baseData.Limit);
    }

    public int Serenity
    {
        get => serenity;
        set => serenity = Mathf.Clamp(value, -baseData.Limit, baseData.Limit);
    }

    public int Energy
    {
        get => energy;
        set => energy = Mathf.Clamp(value, -baseData.Limit, baseData.Limit);
    }

    public int Hearts
    {
        get => hearts;
        set => hearts = Mathf.Clamp(value, 0, baseData.HeartsBeforeLeaving);
    }

    public InhabitantInstance(Inhabitant data, HouseObject _houseObject)
    {
        baseData = data;
        Mood = data.Mood;
        Serenity = data.Serenity;
        Energy = data.Energy;
        Hearts = data.Hearts;
        houseObject = _houseObject;
        _houseObject.inhabitantInstance = this;
    }

    public InhabitantInstance() { }
    
    public void DiscoverInterest(InterestCategory category)
    {
        if (baseData.Likes.Contains(category) && !DiscoveredLikes.Contains(category))
            DiscoveredLikes.Add(category);

        if (baseData.Dislikes.Contains(category) && !DiscoveredDislikes.Contains(category))
            DiscoveredDislikes.Add(category);
    }

    public int IsInterestLiked(InterestCategory category)
    {
        if (DiscoveredLikes.Contains(category))
            return 1;
        else if (DiscoveredDislikes.Contains(category))
            return -1;
        else
            return 0;
    }



    public string Name => baseData.Name;
    public Sprite Icon => baseData.Icon;
    public List<InterestCategory> Likes => baseData.Likes;
    public List<InterestCategory> Dislikes => baseData.Dislikes;

    public float GoldMultiplier => baseData.GoldMultiplier;



    public void FinishActivity(List<Building.AttributeEffect> _attributes, int _energy, int _mood, int _serenity)
    {
        isInActivity = false;

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
        public List<string> discoveredLikes = new();
        public List<string> discoveredDislikes = new();

        public Vector3Int housePos;
    }

    public SavePartData Serialize()
    {
        var data = new SavePartData();

        data.baseInhabitantName = baseData.Name;

        data.mood = Mood;
        data.serenity = Serenity;
        data.energy = energy;
        data.hearts = Hearts;


        foreach (var like in DiscoveredLikes)
        {
            data.discoveredLikes.Add(like.interestName);
        }
        foreach (var dislike in DiscoveredDislikes)
        {
            data.discoveredDislikes.Add(dislike.interestName);
        }

        data.housePos = houseObject.gameObject.GetComponent<PlaceableObject>().OriginalPosition;

        return data;
    }

    public void Deserialize(SavePartData data)
    {
        baseData = GM.Instance.GetInhabitantByName(data.baseInhabitantName);
        Mood = data.mood;
        Serenity = data.serenity;
        Energy = data.energy;
        Hearts = data.hearts;

        foreach (var like in data.discoveredLikes)
        {
            InterestCategory element = GM.DMM.interestDatabase.GetInterestByName(like);
            if (element != null)
            {
                DiscoveredLikes.Add(element);
            }
            else Debug.LogError("Interest not found: " + like);
        }
        foreach (var dislike in data.discoveredDislikes)
        {
            InterestCategory element = GM.DMM.interestDatabase.GetInterestByName(dislike);
            if (element != null)
            {
                DiscoveredDislikes.Add(element);
            }
            else Debug.LogError("Interest not found: " + dislike);
        }

        houseObject.inhabitantInstance = this;

        houseObject.GetComponent<PlaceableObject>().OriginalPosition = data.housePos;
        houseObject.GetComponent<PlaceableObject>().ResetPosition();
    }
}
