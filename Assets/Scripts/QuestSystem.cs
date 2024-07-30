using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public int questID;
    public string questTitle;
    public string questDescription;
    public List<QuestObjective> objectives;
    public List<QuestReward> rewards;
    public bool isHidden;
    public bool isCompleted;
    public bool hasCollectedReward;

    // Constructor de Quest
    public Quest(int id, string title, string description, bool hidden)
    {
        questID = id;
        questTitle = title;
        questDescription = description;
        objectives = new List<QuestObjective>();
        rewards = new List<QuestReward>();
        isHidden = hidden;
        isCompleted = false;
        hasCollectedReward = false;
    }

    // Verificar si la misión está completada
    public bool CheckCompletion()
    {
        foreach (QuestObjective objective in objectives)
        {
            if (!objective.isCompleted)
                return false;
        }
        return true;
    }
}

[System.Serializable]
public class QuestObjective
{
    public string objectiveDescription;
    public int requiredAmount;
    public int currentAmount;
    public bool isCompleted;

    // Constructor de QuestObjective
    public QuestObjective(string description, int amount)
    {
        objectiveDescription = description;
        requiredAmount = amount;
        currentAmount = 0;
        isCompleted = false;
    }
}

[System.Serializable]
public class QuestReward
{
    public enum RewardType { Experience, Money, Item, Access };
    public RewardType rewardType;
    public int amount;
    public string rewardDescription;
    public string itemName; // Propiedad para el nombre del item

    // Constructor por defecto
    public QuestReward()
    {
    }

    // Constructor para recompensas de tipo Item
    public QuestReward(RewardType type, string description, string item)
    {
        rewardType = type;
        rewardDescription = description;
        itemName = item;
    }

    // Constructor para recompensas sin itemName
    public QuestReward(RewardType type, int value, string description)
    {
        rewardType = type;
        amount = value;
        rewardDescription = description;
        itemName = null; // Establecer itemName como null para recompensas que no sean de tipo Item
    }
}

public class QuestSystem : MonoBehaviour, ISaveable
{
    public string ID => GetInstanceID().ToString(); //SAVELOAD
    public Type SaveType => typeof(QuestSystemSaveData);


    public List<Quest> quests = new List<Quest>();

    // Quest Zero
    Quest quest0 = new Quest(0, "Initial Gift from Tio Juancho", "Receive initial items from Tio Juancho.", true);
    public bool zeroQuestFinished;

    Quest quest1 = new Quest(1, "Getting Meat for Tio Juancho", "Tio Juancho needs A LOT of meat to serve the customers of the Tavern!", false);

    public bool firstQuestActive;
    public bool firstQuestKillFinished;
    public bool firstQuestItemFinished;

    public int yitopoCount;
    public int yotepoCount;
    public int ranitomaCount;
    public int ranotemaCount;
    public int chickenLegCount;
    public int porkHamCount;
    public int baconCount;
    public int honeyInfusedBaconCount;

    Quest quest2 = new Quest(2, "Rivalry between Brothers", "Tio Juanete is inside Tio Juancho's house. Go teach him a lesson.", false);

    public bool secondQuestActive;
    public bool secondQuestKillFinished;

    public int tioJuaneteCount;

    private void Start()
    {
        SaveLoadSystem.Instance.RegisterSaveable(this);
        quests.Add(quest0);
        zeroQuestFinished = false;
    }

    public void CompleteQuestZero()
    {
        zeroQuestFinished = true;
    }

    public void AddFirstQuest()
    {
        // Agregar los objetivos individuales
        quest1.objectives.Add(new QuestObjective("Kill 12 Yitopo", 12));
        quest1.objectives.Add(new QuestObjective("Kill 8 Yotepo", 8));
        quest1.objectives.Add(new QuestObjective("Kill 10 Ranitoma", 10));
        quest1.objectives.Add(new QuestObjective("Kill 6 Ranotema", 6));
        quest1.objectives.Add(new QuestObjective("Give 'NPC Tio Juancho' 20 Chicken Leg", 20));
        quest1.objectives.Add(new QuestObjective("Give 'NPC Tio Juancho' 12 Pork Ham", 12));
        quest1.objectives.Add(new QuestObjective("Give 'NPC Tio Juancho' 6 Bacon", 6));
        quest1.objectives.Add(new QuestObjective("Give 'NPC Tio Juancho' 2 Honey-Infused Bacon", 2));

        // Agregar las recompensas individuales
        quest1.rewards.Add(new QuestReward { rewardType = QuestReward.RewardType.Experience, amount = 500, rewardDescription = "You have gained 500 experience points." });
        quest1.rewards.Add(new QuestReward { rewardType = QuestReward.RewardType.Money, amount = 2000, rewardDescription = "You have received 2000 gold." });
        quest1.rewards.Add(new QuestReward { rewardType = QuestReward.RewardType.Access, rewardDescription = "Access to 'Casa del Tio Juanete' has been granted." });

        quests.Add(quest1); // Agregar la primera quest a la lista de quests
        firstQuestActive = true;
    }

    public void EnemyDefeated(string monsterName)
    {
        Debug.Log("EnemyDefeated succesfully called.");
        // Verificar si la misión está activa y qué monstruo se ha derrotado
        if (firstQuestActive)
        {
            if (monsterName == "Yitopo")
            {
                yitopoCount++;
                FirstQuestKillCountMessage();
            }
            else if (monsterName == "Yotepo")
            {
                yotepoCount++;
                FirstQuestKillCountMessage();
            }
            else if (monsterName == "Ranitoma")
            {
                ranitomaCount++;
                FirstQuestKillCountMessage();
            }
            else if (monsterName == "Ranotema")
            {
                ranotemaCount++;
                FirstQuestKillCountMessage();
            }
        }

        if(secondQuestActive)
        {
            if(monsterName == "Tio Juanete")
            {
                tioJuaneteCount++;
                SecondQuestKillCountMessage();
            }
        }
    }

    public void FirstQuestKillCountMessage()
    {
        // Verificar si se ha completado la primera tarea de los enemigos
            if (firstQuestActive && yitopoCount >= 12 && yotepoCount >= 8 && ranitomaCount >= 10 && ranotemaCount >= 6)
            {
                Debug.Log("You have killed 12 Yitopo, 8 Yotepo, 10 Ranitoma & 6 Ranotema. If you have all the ingredients, go talk to Tio Juancho.");
                firstQuestKillFinished = true;
            }
            else
            {
                Debug.Log("You have killed " + yitopoCount + "/12 Yitopo, " + yotepoCount + "/8 Yotepo, " + ranitomaCount + "/10 Ranitoma & " + ranotemaCount + "/6 Ranotema.");
            }
    }

    public void SecondQuestKillCountMessage()
    {
        // Verificar si se ha completado la primera tarea de los enemigos
            if (secondQuestActive && tioJuaneteCount >= 1)
            {
                Debug.Log("You have defeated Tio Juanete. Go talk to Tio Juancho.");
                secondQuestKillFinished = true;
            }
    }

    public void ItemCollected(string itemName)
    {
        // Verificar qué tipo de elemento se ha recogido y actualizar el contador correspondiente
        if (itemName == "Chicken Leg" && firstQuestActive)
        {
            chickenLegCount++;
            FirstQuestItemCountMessage();
        }
        else if (itemName == "Pork Ham" && firstQuestActive)
        {
            porkHamCount++;
            FirstQuestItemCountMessage();
        }
        else if (itemName == "Bacon" && firstQuestActive)
        {
            baconCount++;
            FirstQuestItemCountMessage();
        }
        else if (itemName == "Honey-Infused Bacon" && firstQuestActive)
        {
            honeyInfusedBaconCount++;
            FirstQuestItemCountMessage();
        }
        else
        {

        }
    }

    public void FirstQuestItemCountMessage()
    {
        // Verificar si se ha completado la primera tarea de los elementos recogidos
            if (firstQuestActive && chickenLegCount >= 20 && porkHamCount >= 12 && baconCount >= 6 && honeyInfusedBaconCount >= 2)
            {
                Debug.Log("You have collected 20 Chicken Legs, 12 Pork Hams, 6 Bacons & 2 Honey-Infused Bacons. If you have killed all the monster, go talk to Tio Juancho.");
                firstQuestItemFinished = true;
            }
            else
            {
                Debug.Log("You have collected " + chickenLegCount + "/20 Chicken Legs, " + porkHamCount + "/12 Pork Hams, " + baconCount + "/6 Bacons & " + honeyInfusedBaconCount + "/2 Honey-Infused Bacon.");
            }
    }

    public void AddSecondQuest()
    {
        // Agregar los objetivos individuales
        quest2.objectives.Add(new QuestObjective("Defeat 1 Tio Juanete", 1));

        // Agregar las recompensas individuales
        quest2.rewards.Add(new QuestReward { rewardType = QuestReward.RewardType.Experience, amount = 750, rewardDescription = "You have gained 750 experience points." });
        quest2.rewards.Add(new QuestReward { rewardType = QuestReward.RewardType.Money, amount = 3000, rewardDescription = "You have received 3000 gold." });
        quest2.rewards.Add(new QuestReward { rewardType = QuestReward.RewardType.Item, rewardDescription = "You have received an Old Pendant.", itemName = "Old Pendant" });

        quests.Add(quest2); // Agregar la segunda quest a la lista de quests
        secondQuestActive = true;
    }

    // Obtener una misión por ID
    public Quest GetQuestByID(int questID)
    {
        foreach (Quest quest in quests)
        {
            if (quest.questID == questID)
                return quest;
        }
        return null;
    }

    // Añadir progreso a una misión específica
    public void AddQuestProgress(int questID, string objectiveDescription, int amount)
    {
        Quest quest = GetQuestByID(questID);
        if (quest != null)
        {
            foreach (QuestObjective objective in quest.objectives)
            {
                if (objective.objectiveDescription == objectiveDescription)
                {
                    objective.currentAmount += amount;
                    if (objective.currentAmount >= objective.requiredAmount)
                    {
                        objective.currentAmount = objective.requiredAmount;
                        objective.isCompleted = true;
                    }
                    break;
                }
            }
        }
    }

    // Completar una misión
    public void CompleteQuest(int questID)
    {
        Quest quest = GetQuestByID(questID);
        if (quest != null)
        {
            if (!quest.isCompleted && quest.CheckCompletion())
            {
                quest.isCompleted = true;
                // Otorgar recompensas
                foreach (QuestReward reward in quest.rewards)
                {
                    GiveQuestReward(reward);
                }
            }
        }
    }

    // Otorgar recompensas
    private void GiveQuestReward(QuestReward reward)
    {
        switch (reward.rewardType)
        {
            case QuestReward.RewardType.Experience:
                // Otorgar experiencia al jugador
                break;
            case QuestReward.RewardType.Money:
                // Otorgar dinero al jugador
                break;
            case QuestReward.RewardType.Item:
                // Otorgar objeto al jugador
                break;
            case QuestReward.RewardType.Access:
                // Otorgar acceso al jugador
                break;
        }
    }

    // Recoger recompensa
    public void CollectQuestReward(int questID)
    {
        Quest quest = GetQuestByID(questID);
        if (quest != null)
        {
            if (quest.isCompleted && !quest.hasCollectedReward)
            {
                // Marcar la recompensa como recogida
                quest.hasCollectedReward = true;
                // Realizar cualquier acción adicional requerida al recoger la recompensa
            }
        }
    }

    public object SaveState()
    {
        QuestSystemSaveData saveData = new QuestSystemSaveData();

        // Guardar datos de quests
        foreach (Quest quest in quests)
        {
            QuestSaveData questData = new QuestSaveData(quest.questID, quest.questTitle, quest.isCompleted, quest.hasCollectedReward);
            saveData.quests.Add(questData);
        }

        Debug.Log("QuestSystem SaveState called. Data to be saved: \n" +
                $"Total Quests: {saveData.quests.Count}, \n" +
                $"zeroQuestFinished: {saveData.zeroQuestFinished}, \n" +
                $"firstQuestActive: {saveData.firstQuestActive}, \n" +
                $"firstQuestKillFinished: {saveData.firstQuestKillFinished}, \n" +
                $"firstQuestItemFinished: {saveData.firstQuestItemFinished}, \n" +
                $"yitopoCount: {saveData.yitopoCount}, \n" +
                $"yotepoCount: {saveData.yotepoCount}, \n" +
                $"ranitomaCount: {saveData.ranitomaCount}, \n" +
                $"ranotemaCount: {saveData.ranotemaCount}, \n" +
                $"chickenLegCount: {saveData.chickenLegCount}, \n" +
                $"porkHamCount: {saveData.porkHamCount}, \n" +
                $"baconCount: {saveData.baconCount}, \n" +
                $"honeyInfusedBaconCount: {saveData.honeyInfusedBaconCount}, \n" +
                $"secondQuestActive: {saveData.secondQuestActive}, \n" +
                $"secondQuestKillFinished: {saveData.secondQuestKillFinished}, \n" +
                $"tioJuaneteCount: {saveData.tioJuaneteCount}");

        for (int i = 0; i < saveData.quests.Count; i++)
        {
            var quest = saveData.quests[i];
            Debug.Log($"Quest {i + 1}: ID = {quest.questID}, Title = {quest.questTitle}, IsCompleted = {quest.isCompleted}, HasCollectedReward = {quest.hasCollectedReward}");
        }

        // Guardar otras variables
        saveData.zeroQuestFinished = zeroQuestFinished;
        saveData.firstQuestActive = firstQuestActive;
        saveData.firstQuestKillFinished = firstQuestKillFinished;
        saveData.firstQuestItemFinished = firstQuestItemFinished;
        saveData.yitopoCount = yitopoCount;
        saveData.yotepoCount = yotepoCount;
        saveData.ranitomaCount = ranitomaCount;
        saveData.ranotemaCount = ranotemaCount;
        saveData.chickenLegCount = chickenLegCount;
        saveData.porkHamCount = porkHamCount;
        saveData.baconCount = baconCount;
        saveData.honeyInfusedBaconCount = honeyInfusedBaconCount;
        saveData.secondQuestActive = secondQuestActive;
        saveData.secondQuestKillFinished = secondQuestKillFinished;
        saveData.tioJuaneteCount = tioJuaneteCount;

        return saveData;
    }



    public void LoadState(object state)
{
    if (state is QuestSystemSaveData saveData)
    {
        // Cargar datos de quests
        foreach (QuestSaveData questData in saveData.quests)
        {
            Quest quest = GetQuestByID(questData.questID);
            if (quest != null)
            {
                quest.isCompleted = questData.isCompleted;
                quest.hasCollectedReward = questData.hasCollectedReward;
            }
        }

        // Cargar otras variables
        zeroQuestFinished = saveData.zeroQuestFinished;
        firstQuestActive = saveData.firstQuestActive;
        firstQuestKillFinished = saveData.firstQuestKillFinished;
        firstQuestItemFinished = saveData.firstQuestItemFinished;
        yitopoCount = saveData.yitopoCount;
        yotepoCount = saveData.yotepoCount;
        ranitomaCount = saveData.ranitomaCount;
        ranotemaCount = saveData.ranotemaCount;
        chickenLegCount = saveData.chickenLegCount;
        porkHamCount = saveData.porkHamCount;
        baconCount = saveData.baconCount;
        honeyInfusedBaconCount = saveData.honeyInfusedBaconCount;
        secondQuestActive = saveData.secondQuestActive;
        secondQuestKillFinished = saveData.secondQuestKillFinished;
        tioJuaneteCount = saveData.tioJuaneteCount;
    }
    else
    {
        Debug.LogError("Failed to load QuestSystem state: Invalid save data type");

    }
    
}

}

[Serializable]
public class QuestSystemSaveData
{
    public List<QuestSaveData> quests;
    public bool zeroQuestFinished;
    public bool firstQuestActive;
    public bool firstQuestKillFinished;
    public bool firstQuestItemFinished;
    public int yitopoCount;
    public int yotepoCount;
    public int ranitomaCount;
    public int ranotemaCount;
    public int chickenLegCount;
    public int porkHamCount;
    public int baconCount;
    public int honeyInfusedBaconCount;
    public bool secondQuestActive;
    public bool secondQuestKillFinished;
    public int tioJuaneteCount;

    // Constructor vacío
    public QuestSystemSaveData()
    {
        quests = new List<QuestSaveData>();
    }
}

[Serializable]
public class QuestSaveData
{
    public int questID;
    public string questTitle;
    public bool isCompleted;
    public bool hasCollectedReward;

    // Constructor
    public QuestSaveData(int id, string title, bool completed, bool collectedReward)
    {
        questID = id;
        questTitle = title;
        isCompleted = completed;
        hasCollectedReward = collectedReward;
    }
}
