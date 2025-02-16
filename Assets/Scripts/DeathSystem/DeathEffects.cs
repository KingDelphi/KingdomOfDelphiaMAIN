using UnityEngine;
using System.Collections;

public class DeathEffects : MonoBehaviour
{
    private Animator animator;
    private CharacterController characterController;
    private Collider playerCollider;
    private Rigidbody playerRigidbody;
    private DeathSystem deathSystem;

    void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        playerCollider = GetComponent<Collider>();
        playerRigidbody = GetComponent<Rigidbody>();
        deathSystem = GetComponent<DeathSystem>();
    }

    void Start()
    {
        if (deathSystem != null)
        {
            deathSystem.OnDeathUI += HandleDeathEffects;
        }
    }

    private void HandleDeathEffects()
    {
        // Buscar el script de animación en los hijos del personaje
        DeathAnimationController deathAnim = GetComponentInChildren<DeathAnimationController>();

        if (deathAnim != null)
        {
            deathAnim.PlayDeathAnimation();
        }
        else
        {
            Debug.LogError("No se encontró DeathAnimationController en los hijos.");
        }
        

        DisablePlayerControls();
    }

    private void DisablePlayerControls()
    {
        if (characterController != null) characterController.enabled = false;
        if (playerCollider != null) playerCollider.enabled = false;
        if (playerRigidbody != null) playerRigidbody.isKinematic = true;
    }

    public void ReviveEffects()
    {
        if (animator != null) animator.enabled = true;
        if (characterController != null) characterController.enabled = true;
        if (playerCollider != null) playerCollider.enabled = true;
        if (playerRigidbody != null) playerRigidbody.isKinematic = false;
    }
}
