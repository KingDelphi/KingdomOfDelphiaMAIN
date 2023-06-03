using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float displayDuration = 1f;

    private TextMeshPro textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void ShowDamageText(int damage)
    {
        textMesh.text = damage.ToString();
        Invoke("HideDamageText", displayDuration);
    }

    private void HideDamageText()
    {
        gameObject.SetActive(false);
    }
}
