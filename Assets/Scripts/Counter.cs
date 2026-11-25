using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour, ISaveable
{
    public string ID =>  GetEntityId().ToString(); //SAVELOAD
    public Type SaveType => typeof(CounterSaveData); // Tipo de estado guardado

    QuestSystem questSystem;
    public int yitopoCount;
    public int yotepoCount;
    public int ranitomaCount;
    public int ranotemaCount;
    
    // Start is called before the first frame update
    void Start()
    {
        SaveLoadSystem.Instance.RegisterSaveable(this);
        // Buscar el componente QuestSystem en el objeto actual o en sus padres
        questSystem = GetComponentInParent<QuestSystem>();
        
        if (questSystem == null)
        {
            Debug.LogError("QuestSystem no encontrado. Asegúrate de que el objeto o uno de sus padres tenga el componente QuestSystem.");
        }
    }

    // Método para llamar a EnemyDefeated, tanto del Counter como del QuestSystem, cuando se mate a un monstruo
    public void MonsterKilled(string monsterName)
    {
        EnemyDefeated(monsterName);
        questSystem.EnemyDefeated(monsterName);
    }

    public void EnemyDefeated(string monsterName)
    {
        if (monsterName == "Yitopo")
        {
            yitopoCount++;
        }
        else if (monsterName == "Yotepo")
        {
            yotepoCount++;
        }
        else if (monsterName == "Ranitoma")
        {
            ranitomaCount++;
        }
        else if (monsterName == "Ranotema")
        {
            ranotemaCount++;
        }
    }

    // Método para llamar a ItemCollected del QuestSystem, cuando se obtiene un item de quest
    public void ItemCollected(string itemName)
    {
        questSystem.ItemCollected(itemName);
    }

        // Comienza Implementación de ISaveable en el script Counter
     // Implementación de ISaveable: Guardar estado
    public object SaveState()
    {
        var saveData = new CounterSaveData
        {
            yitopoCount = yitopoCount,
            yotepoCount = yotepoCount,
            ranitomaCount = ranitomaCount,
            ranotemaCount = ranotemaCount
        };

        Debug.Log($"Counter SaveState called. Data to be saved: \n" +
                $"yitopoCount: {saveData.yitopoCount}, \n" +
                $"yotepoCount: {saveData.yotepoCount}, \n" +
                $"ranitomaCount: {saveData.ranitomaCount}, \n" +
                $"ranotemaCount: {saveData.ranotemaCount}");

        return saveData;
    }

    // Implementación de ISaveable: Cargar estado
    public void LoadState(object state)
    {
        // Convertir el objeto de estado al tipo correcto y asignar los valores
        if (state is CounterSaveData data)
        {
            yitopoCount = data.yitopoCount;
            yotepoCount = data.yotepoCount;
            ranitomaCount = data.ranitomaCount;
            ranotemaCount = data.ranotemaCount;
        }
        else
        {
            Debug.LogError($"Estado de carga inválido para Counter. Tipo recibido: {state.GetType()}");
        }
    }
}

// Clase de datos para guardar el estado de Counter
[System.Serializable]
public class CounterSaveData
{
    public int yitopoCount;
    public int yotepoCount;
    public int ranitomaCount;
    public int ranotemaCount;
}
        // Termina Implementación de ISaveable en el script Counter
