using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.VersionControl.Message;
using static UnityEngine.EventSystems.EventTrigger;

public class InhabitantInstance
{
    public Inhabitant baseData;
    private int mood;
    private int serenity;
    private int energy;
    private int hearts;


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

    public string FirstName => baseData.FirstName;
    public string LastName => baseData.LastName;
    public Sprite Icon => baseData.Icon;
    public List<InterestCategory> Likes => baseData.Likes;
    public List<InterestCategory> Dislikes => baseData.Dislikes;
}
