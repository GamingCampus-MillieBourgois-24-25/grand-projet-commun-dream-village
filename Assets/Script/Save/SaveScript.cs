using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

static public class SaveScript
{
    private static string savePath = Application.persistentDataPath + "/Saves";
    private static string extension = ".dat"; // Extension du fichier de sauvegarde
    private static string key = "Kwaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // Cl� de 16, 24 ou 32 caract�res


    static private void SaveFile(string dataJson, string fileName)
    {
        byte[] data = Encrypt(dataJson);

        string path = savePath + '/' + fileName + extension;

        if (!File.Exists(path))
        {
            if (Directory.Exists(savePath) == false)
            {
                Directory.CreateDirectory(savePath);
            }

            File.Create(path).Close();
        }

        // �crire dans le fichier s'il existe
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
        {
            fs.Write(data, 0, data.Length);
        }

    }

    static private string LoadFile(string fileName)
    {

        string path = savePath + '/' + fileName + extension;

        byte[] data = File.ReadAllBytes(path);
        string dataStringLoad = Decrypt(data, key);

        if (string.IsNullOrEmpty(dataStringLoad))
        {
            Debug.LogError("Erreur lors du chargement des donn�es. Le fichier peut �tre corrompu.");
            Application.Quit();
            return null;
        }

        return dataStringLoad;
    }




    static private byte[] Encrypt(string _data)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16];

            using (var encryptor = aes.CreateEncryptor())
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(_data);

                Debug.Log("Encrypted : " + _data);
                return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }
        }
    }

    static private string Decrypt(byte[] data, string key)
    {
        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[16]; // Assurez-vous que l'IV est celui attendu par votre chiffrement

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(data, 0, data.Length);
                    string decryptedText = Encoding.UTF8.GetString(decryptedBytes);
                    Debug.Log("Decrypted : " + decryptedText);
                    return decryptedText;
                }
            }
        }
        catch (CryptographicException ex)
        {
            Debug.LogError("Erreur de d�cryption : les donn�es peuvent �tre corrompues. " + ex.Message);
            // Vous pouvez g�rer l'erreur ici (retourner une valeur par d�faut, lever une exception, etc.)
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError("Erreur inattendue lors de la d�cryption : " + ex.Message);
            return null;
        }
    }

    public static void Save<Data>(this ISaveable<Data> saveable, string fileName) where Data : ISaveData
    {
        Data data = saveable.Serialize();

        string json = JsonUtility.ToJson(data);
        SaveFile(json, fileName);
    }

    public static void Load<Data>(this ISaveable<Data> saveable, string fileName) where Data : ISaveData
    {
        string json = LoadFile(fileName);
        if (json != null)
        {
            Data saveData = JsonUtility.FromJson<Data>(json);
            saveable.Deserialize(saveData);
        }
    }
}

public interface ISaveable
{

}

public interface ISaveable<Data> : ISaveable where Data : ISaveData
{
    Data Serialize();
    void Deserialize(Data data);
}

public interface ISaveData
{

}