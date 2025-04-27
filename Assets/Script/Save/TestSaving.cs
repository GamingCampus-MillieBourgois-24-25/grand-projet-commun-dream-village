using System.Collections.Generic;
using UnityEngine;

public class TestSaving : MonoBehaviour, ISaveable<TestSaving.SavePartData>
{
    [System.Serializable]
    public class SavePartData : ISaveData
    {
        public string name = "Player";
/*        float floatifloata = 0.5f;
        int intotiti = 456;
        bool boolabool = true;
        List<int> listou = new List<int> { 1, 2, 3, 4, 5 };
        Dictionary<string, InhabitantEnums> dictou = new Dictionary<string, InhabitantEnums>
        {
            { "Key1", InhabitantEnums.INTJ },
            { "Key2", InhabitantEnums.INFP},
            { "Key3", InhabitantEnums.ENTP}
        };*/
    }

    private string name;

    private void Awake()
    {
        /*SaveDataManager.saveData.testSaving = this;
        SaveDataManager.Save();*/
    }


    private void Start()
    {
        name = "PlayerOnStart";

        this.Save("TestSaving");
        this.Load("TestSaving");

    }

    public SavePartData Serialize()
    {
        SavePartData data = new SavePartData();
        data.name = name;

        return data;
    }

    public void Deserialize(SavePartData data)
    {
        name = data.name;
    }
}
