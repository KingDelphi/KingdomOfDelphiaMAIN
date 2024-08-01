using UnityEngine;
using TMPro;

public class PlayerReputationDisplay : MonoBehaviour
{
    public ReputationSystem reputationSystem;
    public TextMeshPro titleText;

    private ReputationSystem.Faction currentFaction;
    private CharacterFaction characterFaction;


    private void Start()
    {
        if (reputationSystem == null)
        {
            reputationSystem = FindObjectOfType<ReputationSystem>();
        }

        if (titleText == null)
        {
            titleText = GetComponent<TextMeshPro>();
        }

        characterFaction = FindObjectOfType<CharacterFaction>();
        if (characterFaction != null)
        {
            characterFaction.FactionChanged += OnFactionChanged;
        }

        // Inicializa con la facción Delphia Guard
        currentFaction = ReputationSystem.Faction.DelphiaGuard;
        UpdateTitleDisplay();
    }

    private void OnFactionChanged(CharacterFaction.Faction newFaction)
    {
        ChangeFaction((ReputationSystem.Faction)newFaction);
    }

    public void UpdateTitleDisplay()
    {
        if (reputationSystem != null && titleText != null)
        {
            int index = reputationSystem.GetCurrentTitleIndex(currentFaction);
            string currentTitle = reputationSystem.GetCurrentTitle(currentFaction);
            titleText.text = $"Rank {index + 1} {currentTitle}"; // Se suma 1 al índice para que sea 1-based en lugar de 0-based
        }
    }

    // Método para cambiar la facción externamente si es necesario
    public void ChangeFaction(ReputationSystem.Faction newFaction)
    {
        currentFaction = newFaction;
        UpdateTitleDisplay();
    }

    private void OnDestroy()
    {
        if (characterFaction != null)
        {
            characterFaction.FactionChanged -= OnFactionChanged;
        }
    }
}