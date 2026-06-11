using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(string animationName)
    {
        if (animator == null)
        {
            Debug.LogError("❌ Animator no asignado en el script de ataque.");
        }
        else
        {
            Debug.Log("✅ Animator encontrado: " + animator.gameObject.name);
        }
        animator.SetBool("isAttacking", true);
        animator.SetTrigger(animationName);
    }
}
