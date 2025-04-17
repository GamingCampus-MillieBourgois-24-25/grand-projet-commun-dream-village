using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class VillageManager : MonoBehaviour, ISaveable<VillageManager.SavePartData>
{
    [Header("Village Data")]
    public List<Inhabitant> baseInhabitants = new List<Inhabitant>();
    public List<Building> baseBuildings = new List<Building>();

    public List<InhabitantInstance> inhabitants { get; private set; } = new List<InhabitantInstance>();
    public List<BuildingObject> buildings { get; private set; } = new List<BuildingObject>();

    [System.Serializable]
    public class SavePartData : ISaveData
    {
        public List<BuildingObject.SavePartData> buildings = new List<BuildingObject.SavePartData>();
        public List<InhabitantInstance.SavePartData> inhabitants = new List<InhabitantInstance.SavePartData>();
    }








    private void Awake()
    {
        // Crée les instances runtime à partir des SO
        foreach (var inhabitant in baseInhabitants)
        {
            inhabitants.Add(new InhabitantInstance(inhabitant));
        }

        // Trouver un élément sur la scene nommé CoffeeShop
        GameObject coffeeShop = GameObject.Find("CoffeeShop");
        if (coffeeShop != null)
        {
            BuildingObject buildingObject = coffeeShop.GetComponent<BuildingObject>();
            if (buildingObject != null)
            {
                buildings.Add(buildingObject);
                baseBuildings.Add(buildingObject.building);
            }
            else
            {
                Debug.LogError("BuildingObject component not found on CoffeeShop.");
            }
        }
        else
        {
            Debug.LogError("CoffeeShop GameObject not found in the scene.");
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

    public InhabitantInstance GetInhabitant(Inhabitant inhabitant)
    {
        foreach (var instance in inhabitants)
        {
            if (instance.baseData == inhabitant)
            {
                return instance;
            }
        }
        return null;
    }






    public SavePartData Serialize()
    {
        SavePartData data = new SavePartData();

        foreach (var building in buildings)
        {
            data.buildings.Add(building.Serialize());
        }
        foreach (var inhabitant in inhabitants)
        {
            data.inhabitants.Add(inhabitant.Serialize());
        }

        return data;
    }

    public void Deserialize(SavePartData data)
    {

        foreach (var buildingData in data.buildings)
        {
            GameObject buildingInstanciate = Instantiate(GM.Instance.GetBuildingByName(buildingData.baseBuildingName).BuildingPrefab, Vector3.zero, Quaternion.identity);

            BuildingObject loadedBuilding = buildingInstanciate.GetComponent<BuildingObject>();
            loadedBuilding.Deserialize(buildingData);
            buildings.Add(loadedBuilding);
            baseBuildings.Add(loadedBuilding.building);
        }
        foreach (var inhabitantData in data.inhabitants)
        {
            //InhabitantInstance loadedInhabitant = new InhabitantInstance();
            //loadedInhabitant.Deserialize(inhabitantData);
            //inhabitants.Add(loadedInhabitant);
            //baseInhabitants.Add(loadedInhabitant.baseData);
        }
    }
}
