using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class VillageManager : MonoBehaviour, ISaveable<VillageManager.SavePartData>
{
    public List<InhabitantInstance> inhabitants { get; private set; } = new List<InhabitantInstance>();
    public List<BuildingObject> buildings { get; private set; } = new List<BuildingObject>();


    [System.Serializable]
    public class SavePartData : ISaveData
    {
        public List<BuildingObject.SavePartData> buildings = new List<BuildingObject.SavePartData>();
        public List<InhabitantInstance.SavePartData> inhabitants = new List<InhabitantInstance.SavePartData>();
    }

    public void CreateInstanceofScriptable<T>(T _item, GameObject _obj) where T : IScriptableElement
    {
        switch (_item)
        {
            case Inhabitant inhabitant:
                inhabitants.Add(new InhabitantInstance(inhabitant, _obj.GetComponent<HouseObject>()));
                Debug.Log($"New inhabitant added: {inhabitant.Name}");
                break;
            case Building building:
                BuildingObject loadedBuilding = _obj.GetComponent<BuildingObject>();
                buildings.Add(loadedBuilding);
                Debug.Log($"New building added: {building.Name}");
                break;
            default:
                Debug.LogError("Unknown type");
                break;
        }
    }

    public void RemoveInstance(GameObject _obj)
    {
        if (_obj.TryGetComponent<BuildingObject>(out BuildingObject buildingObj) && buildings.Count >= 1)
        {
            buildings.Remove(buildingObj);
            Debug.Log($"Building removed: {buildingObj.baseData.Name}");
        }
        else if (_obj.TryGetComponent<HouseObject>(out HouseObject houseObj) && inhabitants.Count >= 1)
        {
            inhabitants.Remove(houseObj.inhabitantInstance);
            Debug.Log($"Inhabitant removed: {houseObj.inhabitantInstance.baseData.Name}");
        }
        else
        {
            Debug.LogError("Unknown object type");
        }
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
            GameObject buildingInstanciate = Instantiate(GM.Instance.GetBuildingByName(buildingData.baseBuildingName).InstantiatePrefab, Vector3.zero, Quaternion.identity);

            BuildingObject loadedBuilding = buildingInstanciate.GetComponent<BuildingObject>();
            loadedBuilding.Deserialize(buildingData);
            buildings.Add(loadedBuilding);
        }
        foreach (var inhabitantData in data.inhabitants)
        {
            //InhabitantInstance loadedInhabitant = new InhabitantInstance();
            //loadedInhabitant.Deserialize(inhabitantData);
            //inhabitants.Add(loadedInhabitant);
            //baseInhabitants.Add(loadedInhabitant.baseData);
        }

        if (data.buildings.Count == 0)
        {
            GameObject buildingInstanciate = Instantiate(GM.Instance.GetBuildingByName("CoffeeShop").InstantiatePrefab, Vector3.zero, Quaternion.identity);
            BuildingObject loadedBuilding = buildingInstanciate.GetComponent<BuildingObject>();
            buildingInstanciate.GetComponent<PlaceableObject>().OriginalPosition = new Vector3Int(0, 6, 0);
            buildingInstanciate.GetComponent<PlaceableObject>().ResetPosition();
            buildings.Add(loadedBuilding);
        }
    }
}
