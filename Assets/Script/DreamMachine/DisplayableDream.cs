using System.Collections.Generic;
using UnityEngine;

public class DisplayableDream : ISaveable<DisplayableDream.SavePartData>
{
    public class SavePartData : ISaveData
    {
        public DreamOption.SavePartData dream;
        public List<string> orderedElementsName;
        public bool isSelected = false;
    }


    public DreamOption dream;
    public List<InterestCategory> orderedElements;
    public bool isSelected = false;

    public DisplayableDream(DreamOption dream, List<InterestCategory> ordered)
    {
        this.dream = dream;
        this.orderedElements = ordered;
    }

    public DisplayableDream() { }

    public SavePartData Serialize()
    {
        SavePartData data = new SavePartData();
        data.dream = dream.Serialize();
        data.orderedElementsName = new List<string>();
        foreach (var element in orderedElements)
        {
            data.orderedElementsName.Add(element.interestName);
        }
        data.isSelected = isSelected;
        return data;
    }

    public void Deserialize(SavePartData data)
    {
        InterestCategory pos = GM.DMM.interestDatabase.GetInterestByName(data.dream.positiveElement);
        InterestCategory neg = GM.DMM.interestDatabase.GetInterestByName(data.dream.negativeElement);
        InterestCategory random = GM.DMM.interestDatabase.GetInterestByName(data.dream.randomElement);
        dream = new DreamOption(pos, neg, random);

        orderedElements = new List<InterestCategory>();

        foreach (var elementName in data.orderedElementsName)
        {
            InterestCategory element = GM.DMM.interestDatabase.GetInterestByName(elementName);
            if (element != null)
            {
                orderedElements.Add(element);
            }
            else Debug.LogError("Interest not found: " + elementName);
        }
        isSelected = data.isSelected;
    }
}
