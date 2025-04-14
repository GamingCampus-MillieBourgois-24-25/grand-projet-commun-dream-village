using System.Collections.Generic;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    [Header("Village Data")]
    public List<Inhabitant> baseInhabitants = new List<Inhabitant>();
    public List<Building> buildings = new List<Building>();

    public List<InhabitantInstance> inhabitants { get; private set; } = new List<InhabitantInstance>();

    private void Awake()
    {
        // Crée les instances runtime à partir des SO
        foreach (var inhabitant in baseInhabitants)
        {
            inhabitants.Add(new InhabitantInstance(inhabitant));
        }
    }

    public void AddInhabitant(Inhabitant newInhabitant)
    {
        baseInhabitants.Add(newInhabitant);
        inhabitants.Add(new InhabitantInstance(newInhabitant));
        Debug.Log($"New inhabitant added: {newInhabitant.FirstName} {newInhabitant.LastName}");
    }

    public void RemoveInhabitant(InhabitantInstance instanceToRemove)
    {
        if (inhabitants.Count > 1)
        {
            baseInhabitants.Remove(instanceToRemove.baseData);
            inhabitants.Remove(instanceToRemove);
            Debug.Log($"Inhabitant removed: {instanceToRemove.FirstName} {instanceToRemove.LastName}");
        }
    }

    public int GetInhabitantCount()
    {
        return inhabitants.Count;
    }
}
