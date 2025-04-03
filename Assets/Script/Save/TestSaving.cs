using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestSaving : MonoBehaviour
{
    string name = "Player";
    float floatifloata = 0.5f;
    int intotiti = 45;
    bool boolabool = true;
    List<int> listou = new List<int> { 1, 2, 3, 4, 5 };
    Dictionary<string, InhabitantEnums> dictou = new Dictionary<string, InhabitantEnums>
    {
        { "Key1", InhabitantEnums.INTJ },
        { "Key2", InhabitantEnums.INFP},
        { "Key3", InhabitantEnums.ENTP}
    };


    private void Awake()
    {

        TestSaving testSaving = new TestSaving();
        string save = JsonUtility.ToJson(testSaving);

        TestSaving testSaving2 = JsonUtility.FromJson<TestSaving>(save);

        Debug.Log(Application.persistentDataPath + "/save.dat");
        if (!File.Exists(Application.persistentDataPath + "/save.dat"))
        {
            //Create file
            File.Create(Application.persistentDataPath + "/save.dat").Close();
        }

        // Save data
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "name", name },
            { "floatifloata", floatifloata },
            { "intotiti", intotiti },
            { "boolabool", boolabool },
            { "listou", listou },
            { "dictou", dictou }
        };

        PlayerSave.SaveData("PlayerData", data);
        //PlayerSave.SaveFile();
    }



    private void Start()
    {
        // Load data
        Dictionary<string, object> loadedData = PlayerSave.LoadData("PlayerData");
        // Display loaded data


        Debug.Log("Loaded Data:");
        foreach (var item in loadedData)
        {
            Debug.Log($"{item.Key}: {item.Value}");
        }
    }
}
