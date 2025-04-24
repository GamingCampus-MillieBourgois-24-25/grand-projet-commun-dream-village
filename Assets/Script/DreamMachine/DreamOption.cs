public class DreamOption : ISaveable<DreamOption.SavePartData>
{
    public class SavePartData : ISaveData
    {
        public string positiveElement;
        public string negativeElement;
        public string randomElement;
    }


    public InterestCategory positiveElement;
    public InterestCategory negativeElement;
    public InterestCategory randomElement;

    public DreamOption(InterestCategory pos, InterestCategory neg, InterestCategory random)
    {
        positiveElement = pos;
        negativeElement = neg;
        randomElement = random;
    }

    public SavePartData Serialize()
    {
        SavePartData data = new SavePartData();
        data.positiveElement = positiveElement.interestName;
        data.negativeElement = negativeElement.interestName;
        data.randomElement = randomElement.interestName;
        return data;
    }

    public void Deserialize(SavePartData data)
    {
    }
}
