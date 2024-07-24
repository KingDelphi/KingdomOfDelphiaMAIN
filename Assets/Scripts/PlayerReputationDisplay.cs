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
            string currentTitle = reputationSystem.GetCurrentTitle(currentFaction);
            titleText.text = currentTitle;
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