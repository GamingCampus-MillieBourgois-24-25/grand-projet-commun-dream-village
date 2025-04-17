using System.Collections.Generic;
using UnityEngine;

public class InhabitantInstance
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

    public InhabitantInstance(Inhabitant data)
    {
        baseData = data;
        Mood = data.Mood;
        Serenity = data.Serenity;
        Energy = data.Energy;
        Hearts = data.Hearts;
    }
    
    public void DiscoverInterest(InterestCategory category)
    {
        if (baseData.Likes.Contains(category))
            DiscoveredLikes.Add(category);

        if (baseData.Dislikes.Contains(category))
            DiscoveredDislikes.Add(category);
    }


    public string FirstName => baseData.FirstName;
    public string LastName => baseData.LastName;
    public Sprite Icon => baseData.Icon;
    public List<InterestCategory> Likes => baseData.Likes;
    public List<InterestCategory> Dislikes => baseData.Dislikes;

    public float GoldMultiplier => baseData.GoldMultiplier;
}
