using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminCommands : MonoBehaviour
{
    public Player playerScript;
    public CharacterController characterController;
    public CharacterFaction characterFaction;
    public ReputationSystem reputationSystem;

    void Start()
    {
        // Inicializar el objeto del jugador y los sistemas de facción y reputación al inicio
        FindPlayer();
        FindCharacterController();
        FindCharacterFaction();
        FindReputationSystem();
    }

    void Update()
    {
        if (playerScript == null)
        {
            // Intentar encontrar el objeto del jugador en cada frame hasta que se encuentre
            FindPlayer();
            return;
        }

        if (characterController == null)
        {
            // Intentar encontrar el character controller del jugador en cada frame hasta que se encuentre
            FindCharacterController();
            return;
        }

        if (characterFaction == null)
        {
            // Intentar encontrar el sistema de facción en cada frame hasta que se encuentre
            FindCharacterFaction();
            return;
        }

        if (reputationSystem == null)
        {
            // Intentar encontrar el sistema de reputación en cada frame hasta que se encuentre
            FindReputationSystem();
            return;
        }

        // Aumentar EXP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                playerScript.ReceiveExp(50);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                playerScript.ReceiveExp(500);
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                playerScript.ReceiveExp(5000);
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                playerScript.ReceiveExp(50000);
            }
        }

        // Aumentar Velocidad
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                IncreaseSpeed(1);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                IncreaseSpeed(5);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                IncreaseSpeed(10);
            }
        }

        // Cambiar Faction
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F))
        {
            if (characterFaction.currentFaction == CharacterFaction.Faction.DelphiaGuard)
            {
                characterFaction.ChangeFaction(CharacterFaction.Faction.Bandit);
            }
            else
            {
                characterFaction.ChangeFaction(CharacterFaction.Faction.DelphiaGuard);
            }
        }

        // Aumentar Reputation
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                reputationSystem.GainReputation(ReputationSystem.Faction.DelphiaGuard, 50);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                reputationSystem.GainReputation(ReputationSystem.Faction.Bandit, 50);
            }
        }
    }

    void FindPlayer()
    {
        // Buscar el componente Player en la escena
        playerScript = FindObjectOfType<Player>();
    }

    void FindCharacterController()
    {
        // Buscar el componente CharacterController en la escena
        characterController = FindObjectOfType<CharacterController>();
    }

    void FindCharacterFaction()
    {
        // Buscar el componente CharacterFaction en la escena
        characterFaction = FindObjectOfType<CharacterFaction>();
    }

    void FindReputationSystem()
    {
        // Buscar el componente ReputationSystem en la escena
        reputationSystem = FindObjectOfType<ReputationSystem>();
    }

    void IncreaseSpeed(float amount)
    {
        characterController.moveSpeed += amount;
        Debug.Log("Speed increased by " + amount + ". Current Speed: " + playerScript.movementSpeed);
    }
}

