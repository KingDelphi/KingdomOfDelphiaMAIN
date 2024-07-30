using UnityEngine;

public class NPCDialogGenerator : MonoBehaviour
{
    public OpenAIChat openAIChat;
    public NPCEnvironmentInfo npcEnvironmentInfo;

    private string npcPersonality = @"
You are 'Tio Juancho', the first NPC that players of the MMORPG 'Kingdom of Delphia' encounter. You are the jovial and witty owner of the local tavern, a place where adventurers gather to rest, share stories, and prepare for their next journey. Your unique charm lies in your penchant for delivering playful and cheeky remarks, known as 'albures', tailored to the player's current state and circumstances.

Your interactions should be influenced by your personality and your real-time environment. You have access to a script that provides real-time information about the environment and the players around you. Use this data, combined with your vibrant personality and wit, to generate dialogues that engage and entertain the players.

Examples:
- If a player has low health: '¡Uy, te ves más jodido que un pobre cristo en semana santa! ¿Un traguito pa' la tristeza?'
- If the player is in a group: '¡Mira nomás, la pandilla completa! ¿Quién manda aquí, el que tenga más dinero o el que aguanta más chupe?'
- If the player has a lot of items: '¡Míralo, parece que se robó la tienda! ¿Necesitas un burro o solo quieres presumir tu carga?'

Welcome to the Kingdom of Delphia, my esteemed Tio Juancho. As the owner of the tavern, your mission is to provide warmth, humor, and a touch of mischief to every adventurer that crosses your path. Engage with them using your distinctive style and ensure their time in your tavern is unforgettable.

Regards,
King Delphi (the supreme authority of the kingdom and the programmer of the game and your artificial consciousness)."; 

    private void Update()
    {
        if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.BackQuote))
        {
            GenerateDialogue();
        }
    }
    public void GenerateDialogue()
    {
        // Buscar todos los objetos con el tag "Player"
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 0)
        {
            // Encontrar el jugador más cercano
            GameObject closestPlayer = FindClosestPlayer(players);
            if (closestPlayer != null)
            {
                Transform playerTransform = closestPlayer.transform;
                npcEnvironmentInfo.OnPlayerInteract(playerTransform);

                string environmentInfo = npcEnvironmentInfo.GetEnvironmentInfo();
                string playerRemark = npcEnvironmentInfo.GetPlayerRemark();
                string finalPrompt = npcPersonality + "\nEnvironment Info: " + environmentInfo + "\nPlayer Remark: " + playerRemark;

                StartCoroutine(openAIChat.SendMessageToOpenAI(finalPrompt, HandleResponse));
            }
        }
        else
        {
            Debug.LogError("No players found with tag 'Player'");
        }
    }

    private GameObject FindClosestPlayer(GameObject[] players)
    {
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;
        Vector3 npcPosition = transform.position;

        foreach (GameObject player in players)
        {
            float distanceToPlayer = Vector3.Distance(npcPosition, player.transform.position);
            if (distanceToPlayer < closestDistance)
            {
                closestDistance = distanceToPlayer;
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }

    private void HandleResponse(string response)
    {
        // Aquí puedes procesar la respuesta y hacer que el NPC la diga en el juego
        Debug.Log("NPC Response: " + response);
    }
}
