using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public string targetSceneName;
    public int targetSceneIndex;
    public Vector3 entryPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                // Obtener el nombre de la escena actual
                collision.GetComponent<Player>().sceneIndex = targetSceneIndex;
                collision.GetComponent<Player>().client.activePlayerSceneIndex = targetSceneIndex;
                GameManager.Instance.ChangeScene(targetSceneIndex, entryPosition);
            }
            else
            {
                Debug.LogError("GameManager instance is missing.");
            }
        }
    }
}