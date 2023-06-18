using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class Client : NetworkBehaviour
{
    List<Player> characters;
    public Player activePlayer;
    public GameObject playerPrefab;

    [Networked]
    public int activePlayerSceneIndex {get; set;}
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCharacter() // Called when user clicks Sign-In button
    {

    }

    public override void Spawned()
    {
        activePlayerSceneIndex = 1;
        DontDestroyOnLoad(gameObject);
        transform.SetParent(GameManager.Instance.clientHolder);
        gameObject.name = "Client " + GetComponent<NetworkObject>().Id.ToString();
        
        if (GameManager.Instance.clients.Contains(this))
        {
            GameManager.Instance.clients.Remove(this);
        }
        GameManager.Instance.clients.Add(this);
        
        Debug.Log("Spawned");
        if(HasStateAuthority)
        {
            Runner.Spawn(
                playerPrefab, 
                new Vector3(0, 1, 0), 
                Quaternion.identity, 
                Runner.LocalPlayer,
                InitializeCharacterBeforeSpawn
                );
                
            // activePlayer.SetupLocalPlayer();
        }
        else
        {
            // activePlayer.SetupOtherPlayer();
            
        }
    }

    private void InitializeCharacterBeforeSpawn(NetworkRunner runner, NetworkObject obj)
    {
        Player player = obj.GetComponent<Player>();
        player.clientId = GetComponent<NetworkObject>().Id;
        // player.gameObject.name = "Character " + player.GetComponent<NetworkObject>().Id.ToString();
        Debug.Log("Player ClientID = " + player.clientId);
        Debug.Log("Object Name = " + player.name);
    }

    public void SetActivePlayer(Player player)
    {
        activePlayer = player;

        if(HasStateAuthority)
        {        
            activePlayer.SetupLocalPlayer();
        }
        else
        {
            activePlayer.SetupOtherPlayer();
        }
    }
    
}
