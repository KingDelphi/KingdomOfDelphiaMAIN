using UnityEngine;

public class ClickTest : MonoBehaviour
{
    private void OnMouseDown()
    {
        Debug.Log("Click detectado en el objeto: " + name);
    }
}
