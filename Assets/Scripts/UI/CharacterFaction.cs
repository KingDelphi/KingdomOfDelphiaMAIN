using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFaction : MonoBehaviour, ISaveable
{
    public string ID => GetInstanceID().ToString(); //SAVELOAD
    public Type SaveType => typeof(CharacterFactionSaveData);
    public enum Faction { DelphiaGuard, Bandit }
    public Faction currentFaction;

    private ReputationSystem reputationSystem;
    public PlayerReputationDisplay reputationDisplay;
    public event System.Action<Faction> FactionChanged;

    public bool isDelphiaGuard => currentFaction == Faction.DelphiaGuard;
    public bool isBandit => currentFaction == Faction.Bandit;

    public object SaveState()
    {
        CharacterFactionSaveData saveData = new CharacterFactionSaveData();
        saveData.currentFaction = currentFaction;

        Debug.Log($"CharacterFaction SaveState called. Data to be saved: \n" +
                $"Current Faction: {saveData.currentFaction}");

        return saveData;
    }


    public void LoadState(object state)
    {
        if (state is CharacterFactionSaveData saveData)
        {
            currentFaction = saveData.currentFaction;
            // Actualizar la interfaz de reputación si está disponible
            if (reputationDisplay != null)
            {
                reputationDisplay.ChangeFaction((ReputationSystem.Faction)currentFaction);
            }
        }
        else
        {
            Debug.LogError("Failed to load CharacterFaction state: Invalid save data type");

        } 
    }

    private void Start()
    {
        // Inicializa la facción del personaje, por defecto es DelphiaGuard
        currentFaction = Faction.DelphiaGuard;
        reputationSystem = FindObjectOfType<ReputationSystem>();
        reputationDisplay = FindObjectOfType<PlayerReputationDisplay>();

    }

    // Método para cambiar la facción del personaje
    public void ChangeFaction(Faction newFaction)
    {
        if (currentFaction != newFaction)
        {
            currentFaction = newFaction;
            OnFactionChanged();
        }
    }

    // Método que se llama cuando la facción cambia
    public void OnFactionChanged()
    {
        Debug.Log("Faction changed to: " + currentFaction);
        FactionChanged?.Invoke(currentFaction);
        
        if (reputationDisplay != null)
        {
            reputationDisplay.ChangeFaction((ReputationSystem.Faction)currentFaction);
        }
        else
        {
            Debug.LogWarning("PlayerReputationDisplay not found. Make sure it's in the scene.");
        }
    }

    // Método para verificar acciones permitidas por facción
    public bool CanPerformAction(string action)
    {
        if (isDelphiaGuard)
        {
            // Acciones permitidas para Delphia's Guard
            if (action == "Protect" || action == "Defend")
            {
                return true;
            }
        }
        else if (isBandit)
        {
            // Acciones permitidas para Bandit
            if (action == "Steal" || action == "Attack")
            {
                return true;
            }
        }
        return false;
    }

    // Ejemplo de cómo cambiar de facción basándose en una acción
    public void PerformAction(string action)
    {
        if (action == "KillInnocent")
        {
            ChangeFaction(Faction.Bandit);
        }
        else if (action == "CompleteQuestForGuards")
        {
            ChangeFaction(Faction.DelphiaGuard);
        }
    }
}

[Serializable]
    public class CharacterFactionSaveData
    {
        public CharacterFaction.Faction currentFaction;
    }