using System.Collections.Generic;
using UnityEngine;

public class VillageManager : MonoBehaviour, ISaveable<VillageManager.SavePartData>
{
    [SerializeField]
    private Vector3 willithDefaultHousePosition = Vector3.zero;
    public List<InhabitantInstance> inhabitants { get; private set; } = new List<InhabitantInstance>();
    public List<BuildingObject> buildings { get; private set; } = new List<BuildingObject>();

    List<DecorationObject> decorations = new List<DecorationObject>();


    [System.Serializable]
    public class SavePartData : ISaveData
    {
        public List<BuildingObject.SavePartData> buildings = new List<BuildingObject.SavePartData>();
        public List<InhabitantInstance.SavePartData> inhabitants = new List<InhabitantInstance.SavePartData>();
        public List<DecorationObject.SavePartData> decorations = new List<DecorationObject.SavePartData>();
    }


    private void Start()
    {
        if(inhabitants.Count == 0)
        {
            Debug.Log("Corrupted village");
            StartCoroutine(GM.Instance.DeleteSaveCoroutine());
        }
    }


    public void SpawnWillith()
    {
        if (inhabitants.Count == 0)
        {
            Transform playerIslandObject = GM.Instance.playerIslandObject;
            GameObject house = GM.Instance.GetInhabitantByName("Willith Warm").InstantiatePrefab;
            GameObject houseInstanciate = Instantiate(house, willithDefaultHousePosition, house.transform.rotation, playerIslandObject);

            CreateInstanceofScriptable(GM.Instance.GetInhabitantByName("Willith Warm"), houseInstanciate);

            GM.Instance.SaveGame();
        }
    }

    public void SpawnBench()
    {
        if(buildings.Count == 0)
        {
            Transform playerIslandObject = GM.Instance.playerIslandObject;
            GameObject bench = GM.Instance.GetBuildingByName("Bench").InstantiatePrefab;
            GameObject benchInstanciate = Instantiate(bench, willithDefaultHousePosition, bench.transform.rotation, playerIslandObject);

            CreateInstanceofScriptable(GM.Instance.GetBuildingByName("Bench"), benchInstanciate);
            benchInstanciate.GetComponent<PlaceableObject>().OriginalPosition = new(-2, -5);


            GM.Instance.SaveGame();
        }
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
            case Decoration decoration:
                DecorationObject loadedDecoration = _obj.GetComponent<DecorationObject>();
                decorations.Add(loadedDecoration);
                Debug.Log($"New decoration added: {decoration.Name}");
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

        GM.Instance.SaveGame();
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
        foreach (var decoration in decorations)
        {
            data.decorations.Add(decoration.Serialize());
        }

        return data;
    }

    public void Deserialize(SavePartData data)
    {
        Transform playerIslandObject = GM.Instance.playerIslandObject;

        foreach (var inhabitantData in data.inhabitants)
        {
            GameObject house = GM.Instance.GetInhabitantByName(inhabitantData.baseInhabitantName).InstantiatePrefab;
            GameObject houseInstanciate = Instantiate(house, Vector3.zero, house.transform.rotation, playerIslandObject);

            InhabitantInstance loadedInhabitant = new InhabitantInstance();
            loadedInhabitant.houseObject = houseInstanciate.GetComponent<HouseObject>();
            loadedInhabitant.Deserialize(inhabitantData);

            inhabitants.Add(loadedInhabitant);
        }


        foreach (var buildingData in data.buildings)
        {
            GameObject building = GM.Instance.GetBuildingByName(buildingData.baseBuildingName).InstantiatePrefab;
            GameObject buildingInstanciate = Instantiate(building, Vector3.zero, building.transform.rotation, playerIslandObject);

            BuildingObject loadedBuilding = buildingInstanciate.GetComponent<BuildingObject>();
            loadedBuilding.Deserialize(buildingData);
            buildings.Add(loadedBuilding);
        }

        foreach (var decorationData in data.decorations)
        {
            GameObject decoration = GM.Instance.GetDecorationByName(decorationData.baseDecorationName).InstantiatePrefab;
            GameObject decorationInstanciate = Instantiate(decoration, Vector3.zero, decoration.transform.rotation, playerIslandObject);

            DecorationObject loadedDecoration = decorationInstanciate.GetComponent<DecorationObject>();
            loadedDecoration.Deserialize(decorationData);
            decorations.Add(loadedDecoration);
        }


    }
}
