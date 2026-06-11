using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public NPCEnvironmentInfo npcEnvironmentInfo;

    private void OnMouseDown()
    {
        // Buscar todos los objetos con el tag "Player"
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 0)
        {
            // Encontrar el jugador más cercano al NPC
            GameObject closestPlayer = FindClosestPlayer(players);
            if (closestPlayer != null)
            {
                Transform playerTransform = closestPlayer.transform;
                npcEnvironmentInfo.OnPlayerInteract(playerTransform);
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
}
