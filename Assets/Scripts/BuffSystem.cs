using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffSystem : MonoBehaviour
{
    private Dictionary<string, Coroutine> activeBuffs = new Dictionary<string, Coroutine>();

    public void ApplyBuff(string buffName, float duration, System.Action<bool> effect)
    {
        if (activeBuffs.ContainsKey(buffName))
        {
            StopCoroutine(activeBuffs[buffName]); // ⛔ Detiene el buff anterior si ya existe
        }

        activeBuffs[buffName] = StartCoroutine(BuffCoroutine(buffName, duration, effect));
    }

    private IEnumerator BuffCoroutine(string buffName, float duration, System.Action<bool> effect)
    {
        effect?.Invoke(true); // ✅ Aplica el efecto
        yield return new WaitForSeconds(duration);
        effect?.Invoke(false); // ⛔ Quita el efecto

        activeBuffs.Remove(buffName);
    }
}
