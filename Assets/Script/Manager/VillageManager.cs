using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    public static VillageManager instance { get; private set; }

    [Header("Village Data")]
    public List<Inhabitant> inhabitants = new List<Inhabitant>();
    public List<Building> buildings = new List<Building>();

    private void Awake()
    {
        //Si jamais faut assurer qu'il y a qu'un seul Village ou Game Manager
        
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

    public void AddInhabitant(Inhabitant newInhabitant)
    {
        inhabitants.Add(newInhabitant);
        Debug.Log($"New inhabitant added: {newInhabitant.FirstName} {newInhabitant.LastName}");
    }

    public void RemoveInhabitant(Inhabitant inhabitant)
    {
        if (inhabitants.Count > 1)
        {
            inhabitants.Remove(inhabitant);
            Debug.Log($"Inhabitant removed: {inhabitant.FirstName} {inhabitant.LastName}");
        }
    }

    public int GetInhabitantCount()
    {
        return inhabitants.Count;
    }
}
