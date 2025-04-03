using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

static public class PlayerSave
{
    private static string savePath = Application.persistentDataPath + "/save.dat";
    private static string key = "Kwaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // Cl� de 16, 24 ou 32 caract�res

    static private string dataStringSave;
    static private string dataStringLoad;


    static public void SaveData<T>(string data)
    {
        // Ajouter le DictionaryKey et le JSON au dataStringSave
        dataStringSave += data;
    }

    static public Dictionary<string, object> LoadData(string DictionaryKey)
    {
        if(string.IsNullOrEmpty(dataStringLoad))
        {
            // Charger le fichier si dataStringLoad est vide
            LoadFile();
        }


        // S�parer les diff�rentes entr�es du dataStringLoad
        string[] entries = dataStringLoad.Split(';');

        foreach (string entry in entries)
        {
            if (string.IsNullOrEmpty(entry)) continue;

            // S�parer la cl� et la valeur JSON
            string[] keyValue = entry.Split(':');
            if (keyValue.Length != 2)
            {
                for (int i = 2; i < keyValue.Length; i++)
                {
                    keyValue[1] += ':' + keyValue[i];
                }
            }

            string key = keyValue[0];
            string jsonData = keyValue[1];

            // V�rifier si la cl� correspond � DictionaryKey
            if (key == DictionaryKey)
            {
                // D�s�rialiser le JSON en SerializableDictionary
                SerializableDictionary serializableDictionary = JsonUtility.FromJson<SerializableDictionary>(jsonData);

                // Convertir SerializableDictionary en Dictionary<string, object>
                Dictionary<string, object> data = new Dictionary<string, object>();
                for (int i = 0; i < serializableDictionary.keys.Count; i++)
                {
                    data.Add(serializableDictionary.keys[i], serializableDictionary.values[i]);
                }

                return data;
            }
        }

        // Retourner un dictionnaire vide si la cl� n'est pas trouv�e
        return new Dictionary<string, object>();
    }

    static public void SaveFile()
    {
        byte[] data = Encrypt(dataStringSave);

        if (!File.Exists(savePath))
        {
            // Cr�er le fichier s'il n'existe pas
            using (FileStream fs = File.Create(savePath))
            {
                fs.Write(data, 0, data.Length);
            }
        }
        else
        {
            // �crire dans le fichier s'il existe
            using (FileStream fs = new FileStream(savePath, FileMode.Open, FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
            }
        }
    }

    static public void LoadFile()
    {
        if (!File.Exists(savePath))
        {
            // Le fichier n'existe pas, on ne peut pas charger les donn�es
            Debug.LogError("Le fichier de sauvegarde n'existe pas.");
            Application.Quit();
            return;
        }
        else
        {
            byte[] data = File.ReadAllBytes(savePath);
            dataStringLoad = Decrypt(data, key);

            if(string.IsNullOrEmpty(dataStringLoad))
            {
                Debug.LogError("Erreur lors du chargement des donn�es. Le fichier peut �tre corrompu.");
                Application.Quit();
                return;
            }
        }
    }




    static private byte[] Encrypt(string _data)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16];

            using (var encryptor = aes.CreateEncryptor())
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(dataStringSave);

                Debug.Log("Encrypted : " + dataStringSave);
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
}
