using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FactionReputationDataSaveData
{
    public List<ReputationSystem.FactionReputationData> factionReputations = new List<ReputationSystem.FactionReputationData>();
}

public class ReputationSystem : MonoBehaviour, ISerializationCallbackReceiver, ISaveable
{
    private void Start()
    {
        SaveLoadSystem.Instance.RegisterSaveable(this);
    }
    
    public string ID =>  GetEntityId().ToString(); //SAVELOAD

    public Type SaveType => typeof(FactionReputationDataSaveData);

    [System.Serializable]
    public class FactionReputationData
    {
        public Faction faction;
        public int reputation;
        [SerializeField] private int currentTitleIndex;
        [SerializeField] public string currentTitleName;

        public int CurrentTitleIndex
        {
            get => currentTitleIndex;
            set
            {
                currentTitleIndex = value;
                currentTitleName = GetTitleName(faction, value);
            }
        }

        public string CurrentTitleName => currentTitleName;
    }

    public enum Faction { DelphiaGuard, Bandit }

    public enum DelphiaGuardTitle
    {
        Recruit, Sentry, Watchman, Defender, Protector, Sergeant,
        Lieutenant, Captain, Major, Colonel, Brigadier, General,
        Commander, Champion, Guardian, Lord, Grandmaster
    }

    public enum BanditTitle
    {
        Thug, Outlaw, Brigand, Marauder, Raider, Highwayman,
        Scoundrel, Ruffian, Desperado, Looter, Pillager, Warlord,
        Cutthroat, BanditKing, DreadPirate, Shadowmaster, Overlord
    }

    [SerializeField]
    private List<FactionReputationData> factionReputations = new List<FactionReputationData>
    {
        new FactionReputationData { faction = Faction.DelphiaGuard, reputation = 0, CurrentTitleIndex = 0 },
        new FactionReputationData { faction = Faction.Bandit, reputation = 0, CurrentTitleIndex = 0 }
    };

    private readonly List<int> reputationThresholds = new List<int>
    {
        50, 150, 300, 500, 750, 1000, 1300, 1600, 2000, 2500, 3000, 3600, 4200, 4900, 5600, 6400, 7200
    };

    private List<string> reputationHistory = new List<string>();

    // Métodos para modificar la reputación
    public void GainReputation(Faction faction, int amount)
    {
        var factionData = factionReputations.Find(data => data.faction == faction);
        if (factionData != null)
        {
            factionData.reputation += amount;
            CheckReputationBenefits(factionData);
            NotifyPlayer($"Gained {amount} {faction} reputation. Total: {factionData.reputation}");
            UpdatePlayerReputationDisplay();
        }
    }

    public void LoseReputation(Faction faction, int amount)
    {
        var factionData = factionReputations.Find(data => data.faction == faction);
        if (factionData != null)
        {
            factionData.reputation -= amount;
            if (factionData.reputation < 0) factionData.reputation = 0;
            CheckReputationBenefits(factionData);
            NotifyPlayer($"Lost {amount} {faction} reputation. Total: {factionData.reputation}");
            UpdatePlayerReputationDisplay();
        }
    }

    // Métodos para verificar y otorgar beneficios según la reputación
    private void CheckReputationBenefits(FactionReputationData factionData)
    {
        for (int i = 0; i < reputationThresholds.Count; i++)
        {
            if (factionData.reputation < reputationThresholds[i])
            {
                factionData.CurrentTitleIndex = i;
                break;
            }
        }

        GrantBenefits(factionData.faction, factionData);
    }

    private void GrantBenefits(Faction faction, FactionReputationData factionData)
    {
        // Aquí puedes agregar la lógica para otorgar beneficios específicos basados en el título
        // Ejemplo:
        if (faction == Faction.DelphiaGuard)
        {
            if (factionData.CurrentTitleIndex >= (int)DelphiaGuardTitle.Sergeant) // Ejemplo: Aumentar estadísticas si el título es mayor o igual a Sergeant
            {
                // Aumentar estadísticas aquí
            }
            if (factionData.CurrentTitleIndex >= (int)DelphiaGuardTitle.Brigadier) // Ejemplo: Desbloquear una misión especial si el título es mayor o igual a Brigadier
            {
                // Desbloquear misión especial aquí
            }
        }
        else if (faction == Faction.Bandit)
        {
            if (factionData.CurrentTitleIndex >= (int)BanditTitle.Highwayman) // Ejemplo: Aumentar estadísticas si el título es mayor o igual a Highwayman
            {
                // Aumentar estadísticas aquí
            }
            if (factionData.CurrentTitleIndex >= (int)BanditTitle.Warlord) // Ejemplo: Desbloquear una misión especial si el título es mayor o igual a Warlord
            {
                // Desbloquear misión especial aquí
            }
        }
    }

    private void NotifyPlayer(string message)
    {
        // Aquí puedes agregar la lógica para notificar al jugador
        Debug.Log(message);
    }

    public static string GetTitleName(Faction faction, int index)
    {
        if (faction == Faction.DelphiaGuard)
        {
            return ((DelphiaGuardTitle)index).ToString();
        }
        else
        {
            return ((BanditTitle)index).ToString();
        }
    }

    public int GetReputation(Faction faction)
    {
        var factionData = factionReputations.Find(data => data.faction == faction);
        return factionData?.reputation ?? 0;
    }

    public List<string> GetReputationHistory()
    {
        return reputationHistory;
    }

    // Métodos para guardar y cargar la reputación del jugador
    public void SaveReputation()
    {
        foreach (var data in factionReputations)
        {
            PlayerPrefs.SetInt($"{data.faction}Reputation", data.reputation);
            PlayerPrefs.SetInt($"{data.faction}TitleIndex", data.CurrentTitleIndex);
        }
        PlayerPrefs.Save();
    }

    public void LoadReputation()
    {
        foreach (var data in factionReputations)
        {
            data.reputation = PlayerPrefs.GetInt($"{data.faction}Reputation", 0);
            data.CurrentTitleIndex = PlayerPrefs.GetInt($"{data.faction}TitleIndex", 0);
            CheckReputationBenefits(data); // Asegura que se apliquen los beneficios al cargar
        }
    }

    // Implementación de ISerializationCallbackReceiver
    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        // Asegura que los índices de títulos actuales sean válidos después de la deserialización
        foreach (var factionData in factionReputations)
        {
            int maxIndex = factionData.faction == Faction.DelphiaGuard 
                ? System.Enum.GetValues(typeof(DelphiaGuardTitle)).Length - 1
                : System.Enum.GetValues(typeof(BanditTitle)).Length - 1;
            
            factionData.CurrentTitleIndex = Mathf.Clamp(factionData.CurrentTitleIndex, 0, maxIndex);
        }
    }

    

    public string GetCurrentTitle(Faction faction)
{
    var factionData = factionReputations.Find(data => data.faction == faction);
    if (factionData != null)
    {
        return GetTitleName(faction, factionData.CurrentTitleIndex);
    }
    return "";
}

public int GetCurrentTitleIndex(Faction faction)
    {
        var factionData = factionReputations.Find(data => data.faction == faction);
        if (factionData != null)
        {
            return factionData.CurrentTitleIndex;
        }
        return 0;
    }

public void UpdatePlayerReputationDisplay()
{
    PlayerReputationDisplay display = FindObjectOfType<PlayerReputationDisplay>();
    if (display != null)
    {
        display.UpdateTitleDisplay();
    }
}

    public object SaveState()
    {
        FactionReputationDataSaveData saveData = new FactionReputationDataSaveData();

        foreach (var factionData in factionReputations)
        {
            FactionReputationData data = new FactionReputationData();
            data.faction = factionData.faction;
            data.reputation = factionData.reputation;
            data.CurrentTitleIndex = factionData.CurrentTitleIndex;
            data.currentTitleName = factionData.currentTitleName;

            saveData.factionReputations.Add(data);
        }

        Debug.Log("ReputationSystem SaveState called. Data to be saved: \n" +
                $"Total Faction Reputations: {saveData.factionReputations.Count}");

        for (int i = 0; i < saveData.factionReputations.Count; i++)
        {
            var faction = saveData.factionReputations[i];
            Debug.Log($"Faction {i + 1}: \n" +
                    $"Faction Name = {faction.faction}, \n" +
                    $"Reputation = {faction.reputation}, \n" +
                    $"Current Title Index = {faction.CurrentTitleIndex}, \n" +
                    $"Current Title Name = {faction.currentTitleName}");
        }

        return saveData;
    }

    public void LoadState(object state)
    {
        if (state is FactionReputationDataSaveData saveData)
        {
            // Limpiar datos existentes
            factionReputations.Clear();

            // Cargar datos desde saveData
            foreach (var factionData in saveData.factionReputations)
            {
                FactionReputationData data = new FactionReputationData();
                data.faction = factionData.faction;
                data.reputation = factionData.reputation;
                data.CurrentTitleIndex = factionData.CurrentTitleIndex;
                data.currentTitleName = factionData.currentTitleName;

                factionReputations.Add(data);
            }

            // Actualizar beneficios y visualización de reputación
            foreach (var factionData in factionReputations)
            {
                CheckReputationBenefits(factionData);
            }
            UpdatePlayerReputationDisplay();
        }
        else
        {
            Debug.LogError("Failed to load state: Invalid save data type.");
        }
    }
}