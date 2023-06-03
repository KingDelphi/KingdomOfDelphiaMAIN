using UnityEngine;
using System.Collections;
using TMPro;

public class MonsterAttack : MonoBehaviour
{
    public float attackCooldown = 2f; // Tiempo de espera entre cada ataque en segundos
    public float attackRange = 1.5f; // Rango de ataque
    public float attackDamageMin = 5f; // Daño mínimo del ataque
    public float attackDamageMax = 10f; // Daño máximo del ataque
    public float attackHitChance = 0.8f; // Probabilidad de acertar el ataque (0.0 - 1.0)
    public float criticalHitChance = 0.15f; // Probabilidad de golpe crítico (0.0 - 1.0)
    public GameObject attackHitEffect; // Efecto de ataque exitoso
    public GameObject attackMissEffect; // Efecto de ataque fallido
    public GameObject damageTextPrefab; // Prefab del objeto de TextMeshPro para mostrar el daño recibido
    public Color damageTextColor = Color.red; // Color del texto de daño
    public Color criticalDamageTextColor = Color.magenta; // Color del texto de daño crítico

    private GameObject playerCharacter;
    private bool isAttacking;
    private float attackTimer;
    private GameObject damageTextObject;

    public delegate void AttackAction();
    public static event AttackAction OnAttack;

    private Animator animator; // Agrega esta variable

    public bool hasRunAnimation = false; // Agrega esta variable

    public enum AttackType
    {
        Physical,
        Demon,
        Holly
    }

    public AttackType attackType;

    private void Start()
    {
        playerCharacter = GameObject.FindGameObjectWithTag("Player");
        isAttacking = false;
        attackTimer = attackCooldown;
        animator = GetComponent<Animator>(); // Obtén el componente Animator adjunto al objeto

    }

    private void Update()
    {
        if (!isAttacking)
        {
            // Verificar la distancia entre el monstruo y el jugador
            float distanceToPlayer = Vector3.Distance(transform.position, playerCharacter.transform.position);

            if (distanceToPlayer <= attackRange)
            {
                if (hasRunAnimation && animator != null && animator.GetBool("Run"))
                {
                    animator.SetBool("Run", false);
                }
                
                // Si el jugador está dentro del rango de ataque y ha pasado el tiempo de espera, realizar el ataque
                if (attackTimer >= attackCooldown)
                {
                    PerformAttack();
                    attackTimer = 0f;
                }
                else
                {
                    attackTimer += Time.deltaTime;
                }
            }
        }
    }

    private void PerformAttack()
    {
        isAttacking = true;

        // Llamar al evento OnAttack para notificar a los scripts específicos del monstruo
        OnAttack?.Invoke();

        // Calcular el daño del ataque basado en un rango aleatorio
        float attackDamage = Random.Range(attackDamageMin, attackDamageMax);

        // Verificar si el ataque golpea o falla basado en la probabilidad de acierto
        bool attackHits = Random.value <= attackHitChance;

        if (attackHits)
        {
            // Verificar si el ataque es un golpe crítico
            bool isCriticalHit = Random.value <= criticalHitChance;
            if (isCriticalHit)
            {
                // Si es un golpe crítico, incrementar el daño
                attackDamage *= 1.3f;
            }

            // Obtener la defensa correspondiente al tipo de ataque del monstruo
            int defense = 0;
            switch (attackType)
            {
                case AttackType.Physical:
                    defense = playerCharacter.GetComponent<Player>().physicalDefense;
                    break;
                case AttackType.Demon:
                    defense = playerCharacter.GetComponent<Player>().demonDefense;
                    break;
                case AttackType.Holly:
                    defense = playerCharacter.GetComponent<Player>().hollyDefense;
                    break;
            }

            // Aplicar la reducción de daño según la defensa
            attackDamage -= defense;

            // Asegurarse de que el daño no sea negativo
            attackDamage = Mathf.Max(attackDamage, 0f);

            // Si el ataque acierta, aplicar el daño al jugador
            Player player = playerCharacter.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage((int)attackDamage);
            }

            // Reproducir animación B en el punto intermedio entre el monstruo y el jugador
            Vector3 midPoint = (transform.position + playerCharacter.transform.position) * 0.5f;
            GameObject attackHitEffectInstance = Instantiate(attackHitEffect, midPoint, Quaternion.identity);

            // Calcular la posición vertical del texto de daño por encima del personaje
            Vector3 characterHeight = playerCharacter.transform.localScale;
            float yOffset = characterHeight.y;
            Vector3 damageTextPosition = playerCharacter.transform.position + Vector3.up * yOffset + new Vector3(0f, 2.5f, 0f);

            // Instanciar el objeto de texto de daño y almacenar la referencia en damageTextObject
            damageTextObject = Instantiate(damageTextPrefab, damageTextPosition, Quaternion.identity);

            // Obtener el componente TextMeshPro del objeto de texto de daño
            TextMeshPro damageText = damageTextObject.GetComponent<TextMeshPro>();

            // Establecer el color del texto de daño
            damageText.color = isCriticalHit ? criticalDamageTextColor : damageTextColor;

            // Mostrar el daño en el texto
            damageText.text = ((int)attackDamage).ToString();

            // Iniciar la corutina para animar la posición vertical del texto de daño durante 1 segundo
            StartCoroutine(MoveDamageText(damageTextObject, damageTextPosition + Vector3.up));
        }
        else
        {
            // Reproducir animación A en el punto intermedio entre el monstruo y el jugador
            Vector3 midPoint = (transform.position + playerCharacter.transform.position) * 0.5f;
            Instantiate(attackMissEffect, midPoint, Quaternion.identity);
        }

        StartCoroutine(DestroyDamageTextAfterDelay(damageTextObject, 1f));
        isAttacking = false;
    }

    private IEnumerator MoveDamageText(GameObject damageTextObject, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = .8f;
        Vector3 initialPosition = damageTextObject.transform.position;

        while (elapsedTime < duration && damageTextObject != null) // Verificar si el objeto aún existe
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            damageTextObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        // Destruir el objeto clonado después de 1 segundo
        if (damageTextObject != null)
        {
            Destroy(damageTextObject, .8f);
        }
    }

    private IEnumerator DestroyDamageTextAfterDelay(GameObject damageTextObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (damageTextObject != null)
        {
            Destroy(damageTextObject);
        }
    }

    public void DestroyDamageText()
    {
        if (damageTextObject != null)
        {
            Destroy(damageTextObject);
        }
    }
}
