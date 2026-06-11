using UnityEngine;

public class NPCEnvironmentInfo : MonoBehaviour
{
    private string environmentInfo;
    private string playerRemark;

    public void OnPlayerInteract(Transform player)
    {
        // Recopilar información relevante del jugador
        Player playerComponent = player.GetComponent<Player>();
        
        if (playerComponent != null)
        {
            float playerHealth = playerComponent.currentHealth;
            int maxHealth = playerComponent.hp;
            float playerMana = playerComponent.currentMana;
            int maxMana = playerComponent.mana;
            int experience = playerComponent.experience;
            string playerName = playerComponent.playerName;
            float playerWeight = playerComponent.weight;
            int nearbyPlayers = FindObjectsOfType<Player>().Length;

            environmentInfo = $"Player Name: {playerName}, Player Health: {playerHealth}/{maxHealth}, Player Mana: {playerMana}/{maxMana}, Experience: {experience}, Weight: {playerWeight}, Nearby Players: {nearbyPlayers}";

            // Generar comentarios según la información del jugador
            playerRemark = GenerateRemark(playerHealth, maxHealth, nearbyPlayers, playerWeight);

            // Mostrar el comentario generado
            Debug.Log(playerRemark);
        }
        else
        {
            Debug.LogError("Player component not found on the interacting object.");
        }
    }

    private string GenerateRemark(float health, int maxHealth, int nearbyPlayers, float weight)
    {
        string remark = "";

        if (health <= maxHealth * 0.2)
        {
            remark = "¡Uy, te ves más jodido que un pobre cristo en semana santa! ¿Un traguito pa' la tristeza?";
        }
        else if (nearbyPlayers > 1)
        {
            remark = "¡Mira nomás, la pandilla completa! ¿Quién manda aquí, el que tenga más dinero o el que aguanta más chupe?";
        }
        else if (weight > 50) // Supongamos que 50 es un umbral de peso significativo
        {
            remark = "¡Míralo, parece que se robó la tienda! ¿Necesitas un burro o solo quieres presumir tu carga?";
        }
        else if (health > maxHealth * 0.8)
        {
            remark = "¡Andas bien fresco, eh! ¿Te dieron masajes o qué? Porque te ves listo pa' la acción.";
        }
        else if (nearbyPlayers == 1)
        {
            remark = "¿Qué pasó, solitario? ¿Te dejaron plantado o no aguantaron tu paso?";
        }

        return remark;
    }

    public string GetEnvironmentInfo()
    {
        return environmentInfo;
    }

    public string GetPlayerRemark()
    {
        return playerRemark;
    }
}
