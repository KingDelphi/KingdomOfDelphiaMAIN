using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    QuestSystem questSystem;
    public int yitopoCount;
    public int yotepoCount;
    public int ranitomaCount;
    public int ranotemaCount;
    
    // Start is called before the first frame update
    void Start()
    {
        // Buscar el componente QuestSystem en el objeto actual o en sus padres
        questSystem = GetComponentInParent<QuestSystem>();
        
        if (questSystem == null)
        {
            Debug.LogError("QuestSystem no encontrado. Asegúrate de que el objeto o uno de sus padres tenga el componente QuestSystem.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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

}