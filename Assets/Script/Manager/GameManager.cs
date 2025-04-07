using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables

    public static GameManager instance;

    public List<Inhabitant> inhabitants = new List<Inhabitant>();
    public List<Building> buildings = new List<Building>();

 
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //LoadAllResources();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Load all resources for shop from the Resources folder
    private void LoadAllResources()
    {
        // Load all inhabitants
        Inhabitant[] allInhabitants = Resources.LoadAll<Inhabitant>("ScriptableObject/Inhabitant");
        foreach (Inhabitant inhabitant in allInhabitants)
        {
            inhabitants.Add(inhabitant);
        }
        // Load all buildings
        Building[] allBuildings = Resources.LoadAll<Building>("ScriptableObjet/Building");
        foreach (Building building in allBuildings)
        {
            buildings.Add(building);
        }
    }
}
