using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersUnlockedManager : MonoBehaviour
{
    public List<CharacterInfo> characterInfos; // Lista de información de los personajes

    private Dictionary<string, bool> unlockedCharacters; // Diccionario para almacenar el estado de desbloqueo de los personajes

    // Clase para almacenar la información de cada personaje
    [System.Serializable]
    public class CharacterInfo
    {
        public string characterName; // Nombre del personaje
        public GameObject characterObject; // Objeto del personaje (sprite o modelo 3D)
        public bool unlocked; // Estado de desbloqueo del personaje
    }

    private void Awake()
    {
        // Inicializar el diccionario de personajes desbloqueados
        unlockedCharacters = new Dictionary<string, bool>();

        // Recorrer la lista de información de personajes y agregarlos al diccionario
        foreach (CharacterInfo characterInfo in characterInfos)
        {
            unlockedCharacters.Add(characterInfo.characterName, characterInfo.unlocked);
        }

        // Desbloquear los personajes marcados como "unlocked" en el inspector
        foreach (CharacterInfo characterInfo in characterInfos)
        {
            if (characterInfo.unlocked)
            {
                UnlockCharacter(characterInfo.characterName);
            }
        }
    }

    // Método para desbloquear un personaje
    public void UnlockCharacter(string characterName)
    {
        if (unlockedCharacters.ContainsKey(characterName))
        {
            unlockedCharacters[characterName] = true;

            // Obtener el objeto del personaje correspondiente
            GameObject characterObject = GetCharacterObject(characterName);
            if (characterObject != null)
            {
                characterObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Objeto de personaje no encontrado: " + characterName);
            }
        }
        else
        {
            Debug.LogWarning("Personaje no encontrado en el diccionario: " + characterName);
        }
    }

    // Método para verificar si un personaje está desbloqueado
    public bool IsCharacterUnlocked(string characterName)
    {
        if (unlockedCharacters.ContainsKey(characterName))
        {
            return unlockedCharacters[characterName];
        }

        return false;
    }

    // Método para obtener el objeto del personaje asociado al nombre del personaje
    private GameObject GetCharacterObject(string characterName)
    {
        foreach (CharacterInfo characterInfo in characterInfos)
        {
            if (characterInfo.characterName == characterName)
            {
                return characterInfo.characterObject;
            }
        }

        return null;
    }
}