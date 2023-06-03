using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerAttack : MonoBehaviour
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

    private bool isAttacking;
    private float attackTimer;

    public GameObject selectedMonster;
    public float? distanceToSelectedMonster;

    private Animator animator;

    public enum AttackType
    {
        Physical,
        Demon,
        Holly
    }

    public AttackType attackType;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        selectedMonster = null;
        distanceToSelectedMonster = null;
        isAttacking = false;
        attackTimer = attackCooldown;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) {
            SelectTarget();
        }
        
        if (!isAttacking)
        {
            // Verificar la distancia entre el jugador y el monstruo
            if (selectedMonster != null) {
                distanceToSelectedMonster = Vector3.Distance(transform.position, selectedMonster.transform.position);
            }

            if (selectedMonster != null && distanceToSelectedMonster <= attackRange)
            {
                // Si el monstruo está dentro del rango de ataque y ha pasado el tiempo de espera, realizar el ataque
                if (attackTimer >= attackCooldown /*&& agroActive == true*/)
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

    public void PerformAttack()
    {
        // Obtener la instancia del jugador
        Player player = GetComponent<Player>(); // Asumiendo que el componente del jugador se encuentra en el mismo GameObject
        
        isAttacking = true;

        // Calcular el daño máximo del ataque basado en la estadística correspondiente al tipo de ataque
        float maxAttackDamage = 0f;
        switch (attackType)
        {
            case AttackType.Physical:
                maxAttackDamage = player.GetComponent<Player>().physicalAttack * 3f;
                break;
            case AttackType.Demon:
                maxAttackDamage = player.GetComponent<Player>().demonAttack * 3f;
                break;
            case AttackType.Holly:
                maxAttackDamage = player.GetComponent<Player>().hollyAttack * 3f;
                break;
        }

        // Calcular el daño del ataque basado en un rango aleatorio
        float attackDamage = Random.Range(attackDamageMin, maxAttackDamage);

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

            // Obtener la defensa correspondiente al tipo de ataque del jugador
            int defense = 0;
            switch (attackType)
            {
                case AttackType.Physical:
                    defense = selectedMonster.GetComponent<Monsters>().physicalDefense;
                    break;
                case AttackType.Demon:
                    defense = selectedMonster.GetComponent<Monsters>().demonDefense;
                    break;
                case AttackType.Holly:
                    defense = selectedMonster.GetComponent<Monsters>().hollyDefense;
                    break;
            }

            // Aplicar la reducción de daño según la defensa
            attackDamage -= defense;

            // Asegurarse de que el daño no sea negativo
            attackDamage = Mathf.Max(attackDamage, 0f);

            // Si el ataque acierta, aplicar el daño al monstruo
            Monsters monster = selectedMonster.GetComponent<Monsters>();
            if (monster != null)
            {
                int damageAmount = (int)attackDamage;
                monster.TakeDamage(player, damageAmount);
            }

            // Reproducir animación B en el punto intermedio entre el jugador y el monstruo
            Vector3 midPoint = (transform.position + selectedMonster.transform.position) * 0.5f;
            GameObject attackHitEffectInstance = Instantiate(attackHitEffect, midPoint, Quaternion.identity);
            animator.Play("SlashMeleePaired", 0);

            // Calcular la posición vertical del texto de daño por encima del monstruo
            Collider2D monsterCollider = selectedMonster.GetComponent<Collider2D>();
            Bounds colliderBounds = monsterCollider.bounds;

            float yOffset = 0f;
            float xOffset = 0f;

            Vector3 damageTextPosition;

            if (monsterCollider is CircleCollider2D circleCollider)
            {
                // Para collider circular, calcular el yOffset usando el radio del collider
                yOffset = circleCollider.radius;
            }
            else if (monsterCollider is CapsuleCollider2D capsuleCollider)
            {
                // Para collider en forma de huevo (capsule), calcular el yOffset usando la mitad de la altura del collider
                yOffset = capsuleCollider.size.y * 0.5f;

                // Ajustar el yOffset para que el texto aparezca un poco más abajo en el caso del CapsuleCollider
                yOffset -= 1.5f; // Ajusta este valor según tus necesidades
            }

            // Calcular el desplazamiento en el eje x para posicionar el texto en el centro del personaje
            xOffset = selectedMonster.transform.localScale.x / 2f;

            // Calcular la posición vertical del texto de daño por encima del borde superior del collider del monstruo
            damageTextPosition = monsterCollider.bounds.max + Vector3.up * yOffset + new Vector3(-xOffset, 0.5f, 0f);

            GameObject damageTextObject = Instantiate(damageTextPrefab, damageTextPosition, Quaternion.identity);

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
            // Reproducir animación A en el punto intermedio entre el jugador y el monstruo
            Vector3 midPoint = (transform.position + selectedMonster.transform.position) * 0.5f;
            Instantiate(attackMissEffect, midPoint, Quaternion.identity);
            animator.Play("SlashMeleePaired", 0);
        }

        isAttacking = false;
    }

    private IEnumerator MoveDamageText(GameObject damageTextObject, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = .8f;
        Vector3 initialPosition = damageTextObject.transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            damageTextObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        // Destruir el objeto clonado después de 1 segundo
        Destroy(damageTextObject, .8f);
    }

    void SelectTarget()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null && hit.collider.tag == "Monster")
        {
            selectedMonster = hit.collider.gameObject;

            // Imprimir la posición y la dirección del rayo en la consola
            Debug.Log("Ray Position: " + hit.point);
            Debug.Log("Ray Direction: " + (hit.point - (Vector2)transform.position).normalized);
        }
    }
}