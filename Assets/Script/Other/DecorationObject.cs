using UnityEngine;

public class DecorationObject : MonoBehaviour, ISaveable<DecorationObject.SavePartData>
{
    public void Deserialize(SavePartData data)
    {
        GetComponent<PlaceableObject>().OriginalPosition = data.position;
        GetComponent<PlaceableObject>().ResetPosition();
    }

    public SavePartData Serialize()
    {
        SavePartData data = new SavePartData();
        data.baseDecorationName = baseData.name;
        data.position = GetComponent<PlaceableObject>().OriginalPosition;
        return data;
    }

    public Decoration baseData;

    public class  SavePartData : ISaveData
    {
        public string baseDecorationName;

        public Vector3Int position;
    }
}
