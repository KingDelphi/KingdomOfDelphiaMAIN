using UnityEngine;
using System.Collections;
using TMPro;
using System;
using System.Collections.Generic;

public class Lobin : MonoBehaviour
{
    public string characterName = "Lobin";
    public int maxLevel = 10;
    public int unlockNextCharacterLevel = 10;

    // Probabilidad y rangos de aumento para cada estadística
    public StatIncreaseSettingsInt hpIncreaseSettings;
    public StatIncreaseSettingsInt manaIncreaseSettings;
    public StatIncreaseSettingsFloat movementSpeedIncreaseSettings;
    public StatIncreaseSettingsFloat criticalChanceIncreaseSettings;
    public StatIncreaseSettingsFloat criticalBonusIncreaseSettings;
    public StatIncreaseSettingsInt physicalAttackIncreaseSettings;
    public StatIncreaseSettingsInt demonAttackIncreaseSettings;
    public StatIncreaseSettingsInt hollyAttackIncreaseSettings;
    public StatIncreaseSettingsInt physicalDefenseIncreaseSettings;
    public StatIncreaseSettingsInt demonDefenseIncreaseSettings;
    public StatIncreaseSettingsInt hollyDefenseIncreaseSettings;
    public StatIncreaseSettingsFloat weightIncreaseSettings;
    public StatIncreaseSettingsInt hpRegenIncreaseSettings;
    public StatIncreaseSettingsInt manaRegenIncreaseSettings;
    public StatIncreaseSettingsFloat effectDurationIncreaseSettings;


    private Player player; // Referencia al script "Player"

    public GameObject nextCharacter;
    public GameObject nextCharacter2;

    private GameObject lvlTextObject;
    public GameObject lvlTextPrefab;

    public Animator animator;

    private float eatMeatCooldown = 8f;
    private float eatMeatTimer = 0f;
    private GameObject eatMeatTextObject;
    public GameObject eatMeatTextPrefab;

    private float roarTimer = 0f;
    private GameObject roarTextObject;
    public GameObject roarTextPrefab;

    [SerializeField]
    private GameObject particleSystem1;

    [SerializeField]
    private GameObject particleSystem2;

    [SerializeField]
    private GameObject particleSystem3;

    public int[] experienceRequiredPerLevel = {
        10, 30, 60, 100, 150, 210, 280, 360, 450
    };

    private int currentLevel = 1;
    private int currentExperience = 0;

    private void Start()
    {
        player = FindObjectOfType<Player>(); // Busca el objeto con el script "Player"
        
        // Suscribirse al evento OnAddExperience del script Player
        if (player != null)
        {
            player.OnAddExperience += AddExperience;
        }
        
        // Inicializar el personaje en el nivel 1
        currentLevel = 1;

        // Desbloquear el siguiente personaje si ya se alcanzó el nivel requerido
        if (currentLevel >= unlockNextCharacterLevel && nextCharacter != null)
        {
            nextCharacter.SetActive(true);
            nextCharacter2.SetActive(true);
        }
    }

    private void Update()
    {
        // Obtener la experiencia actual del script "Player" y hacer lo que sea necesario con ella
        if (player != null)
        {
            int playerExperience = player.experience;
            // Realiza las acciones necesarias con la experiencia del jugador obtenida
            Debug.Log("La experiencia actual del jugador es: " + playerExperience);
        }

        // Eat Meat
        if (Input.GetKeyDown(KeyCode.Alpha1) && eatMeatTimer <= 0)
        {
            EatMeat();
        }

        // Actualizar el temporizador de cooldown de Eat Meat
        if (eatMeatTimer > 0f)
        {
            eatMeatTimer -= Time.deltaTime;

            if (eatMeatTimer <= 0f)
            {
                Debug.Log("El cooldown de Eat Meat ha finalizado.");
            }
        }

        // Roar
        if (Input.GetKeyDown(KeyCode.Alpha2) && roarTimer <= 0)
        {
            Roar();
        }

        // Actualizar el temporizador de cooldown de Roar
        if (roarTimer > 0f)
        {
            roarTimer -= Time.deltaTime;

            if (roarTimer <= 0f)
            {
                Debug.Log("El cooldown de Roar ha finalizado.");
            }
        }
    }

    private void EatMeat()
    {
        // Obtener el componente Player del jugador
        Player player = GetComponent<Player>();

        if (player.currentMana >= 5)
        {
            player.ConsumeMana(5);
        }
        else
        {
            return;
        }
        
        if (eatMeatTimer > 0f)
        {
            Debug.Log("¡Eat Meat aún está en cooldown!");
            return;
        }

        // Llamar al método Heal del jugador para curar el HP
        player.Heal(8);

        // Establecer el cooldown de Eat Meat a 8 segundos
        eatMeatTimer = 8f;

        Debug.Log("¡Eat Meat ha sido utilizado! Se ha curado 8 de vida.");

        StartCoroutine(ResetEatMeatCooldown());

        // Calcular la posición vertical del texto de nivel por encima del personaje
            Vector3 characterHeight = player.transform.localScale;
            float yOffset = characterHeight.y;
            Vector3 eatMeatTextPosition = player.transform.position + Vector3.up * yOffset + new Vector3(0f, 2.5f, 0f);

            // Instanciar el objeto de texto de nivel y almacenar la referencia en lvlTextObject
            eatMeatTextObject = Instantiate(eatMeatTextPrefab, eatMeatTextPosition, Quaternion.identity);

            // Obtener el componente TextMeshPro del objeto de texto de nivel
            TextMeshPro eatMeatText = eatMeatTextObject.GetComponent<TextMeshPro>();

            // Establecer el color del texto de nivel
            eatMeatText.color = Color.green;

            // Mostrar el nivel en el texto
            eatMeatText.text = ("¡Yuuuuum!");

            // Iniciar la corutina para animar la posición vertical del texto de nivel durante 1 segundo
            StartCoroutine(MoveEatMeatText(eatMeatTextObject, eatMeatTextPosition + Vector3.up));
    }

    private void Roar()
    {
        // Obtener el componente Player del jugador
        Player player = GetComponent<Player>();
        PlayerAttack playerAttack = GetComponent<PlayerAttack>();

        if (player.currentMana >= 8)
        {
            player.ConsumeMana(8);
        }
        else
        {
            return;
        }

        if (roarTimer > 0f)
        {
            Debug.Log("¡Roar aún está en cooldown!");
            return;
        }

        // Obtener todos los objetos con la etiqueta "Monster" en la escena
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Monster");

        // Realizar el ataque a cada enemigo en el rango
        foreach (GameObject enemyObject in enemies)
        {
            Monsters enemy = enemyObject.GetComponent<Monsters>();
            if (enemy != null)
            {
                float distanceToEnemy = Vector3.Distance(player.transform.position, enemy.transform.position);
                if (distanceToEnemy <= playerAttack.attackRange)
                {
                    Debug.Log("Enemigo seleccionado: " + enemy.name);
                    RoarAttack(enemy);
                }
            }
        }

        // Establecer el cooldown de Roar a 5 segundos
        roarTimer = 5f;

        Debug.Log("¡Roar ha sido utilizado! Lobin golpea a todos a su alrededor.");

        StartCoroutine(ResetRoarCooldown());

        // Calcular la posición vertical del texto de nivel por encima del personaje
            Vector3 characterHeight = player.transform.localScale;
            float yOffset = characterHeight.y;
            Vector3 roarTextPosition = player.transform.position + Vector3.up * yOffset + new Vector3(0f, 2.5f, 0f);

            // Instanciar el objeto de texto de nivel y almacenar la referencia en lvlTextObject
            roarTextObject = Instantiate(roarTextPrefab, roarTextPosition, Quaternion.identity);

            // Obtener el componente TextMeshPro del objeto de texto de nivel
            TextMeshPro roarText = roarTextObject.GetComponent<TextMeshPro>();

            // Establecer el color del texto de nivel
            roarText.color = Color.red;

            // Mostrar el nivel en el texto
            roarText.text = ("¡Roaaar!");

            // Iniciar la corutina para animar la posición vertical del texto de nivel durante 1 segundo
            StartCoroutine(MoveRoarText(roarTextObject, roarTextPosition + Vector3.up));
    }

    private void RoarAttack(Monsters enemy)
    {
        if (enemy != null)
        {
            PlayerAttack playerAttack = GetComponent<PlayerAttack>();

            // Realizar el ataque directamente en el enemigo
            playerAttack.selectedMonster = enemy.gameObject;
            playerAttack.PerformAttack();
        }
        else
        {
            Debug.Log("El enemigo es nulo. No se puede realizar el ataque.");
        }
    }

    private IEnumerator ResetEatMeatCooldown()
    {
        yield return new WaitForSeconds(eatMeatCooldown);

        Debug.Log("El cooldown de Eat Meat ha finalizado.");
    }

    private IEnumerator ResetRoarCooldown()
    {
        yield return new WaitForSeconds(roarTimer);

        Debug.Log("El cooldown de Roar ha finalizado.");
    }

    private void HandleLevelUp(int level)
    {
        Debug.Log("HandleLevelUp succesfully called. DEBERIA MOSTRAR EL TEXTO DE QUE CEREBRITO SUBIO DE LVL!!!");
        if ((level > (currentLevel - 1)) && level <= maxLevel)
        {
            currentLevel = level;

            // Incrementar las estadísticas según las probabilidades y rangos de aumento
            IncreaseStats(ref player.hp, hpIncreaseSettings);
            player.currentHealth = player.hp;
            IncreaseStats(ref player.mana, manaIncreaseSettings);
            IncreaseStats(ref player.movementSpeed, movementSpeedIncreaseSettings);
            IncreaseStats(ref player.criticalChance, criticalChanceIncreaseSettings);
            IncreaseStats(ref player.criticalBonus, criticalBonusIncreaseSettings);
            IncreaseStats(ref player.physicalAttack, physicalAttackIncreaseSettings);
            IncreaseStats(ref player.demonAttack, demonAttackIncreaseSettings);
            IncreaseStats(ref player.hollyAttack, hollyAttackIncreaseSettings);
            IncreaseStats(ref player.physicalDefense, physicalDefenseIncreaseSettings);
            IncreaseStats(ref player.demonDefense, demonDefenseIncreaseSettings);
            IncreaseStats(ref player.hollyDefense, hollyDefenseIncreaseSettings);
            IncreaseStats(ref player.weight, weightIncreaseSettings);
            IncreaseStats(ref player.hpRegen, hpRegenIncreaseSettings);
            IncreaseStats(ref player.manaRegen, manaRegenIncreaseSettings);
            IncreaseStats(ref player.effectDuration, effectDurationIncreaseSettings);

            // Desbloquear el siguiente personaje si se alcanzó el nivel requerido
            if (currentLevel >= unlockNextCharacterLevel && nextCharacter != null)
            {
                nextCharacter.SetActive(true);
            }

            // Calcular la posición vertical del texto de nivel por encima del personaje
            Vector3 characterHeight = player.transform.localScale;
            float yOffset = characterHeight.y;
            Vector3 lvlTextPosition = player.transform.position + Vector3.up * yOffset + new Vector3(0f, 2.5f, 0f);

            // Instanciar el objeto de texto de nivel y almacenar la referencia en lvlTextObject
            lvlTextObject = Instantiate(lvlTextPrefab, lvlTextPosition, Quaternion.identity);

            // Obtener el componente TextMeshPro del objeto de texto de nivel
            TextMeshPro lvlText = lvlTextObject.GetComponent<TextMeshPro>();

            // Establecer el color del texto de nivel
            lvlText.color = Color.white;

            // Mostrar el nivel en el texto
            lvlText.text = ("¡" + characterName + " ha subido de nivel al nivel " + currentLevel + "!");

            // Iniciar la corutina para animar la posición vertical del texto de nivel durante 1 segundo
            StartCoroutine(MoveLvlText(lvlTextObject, lvlTextPosition + Vector3.up));
        }
    }

private void IncreaseStats<T>(ref T playerStat, StatIncreaseSettings<T> settings)
    where T : struct
{
    if (UnityEngine.Random.value <= settings.probability)
    {
        float statIncreaseAmount = UnityEngine.Random.Range(settings.minIncreaseAmount, settings.maxIncreaseAmount + 1);
        if (typeof(T) == typeof(int))
        {
            int roundedIncreaseAmount = Mathf.RoundToInt(statIncreaseAmount);
            playerStat = (T)(object)((int)(object)playerStat + roundedIncreaseAmount);
            Debug.Log("El jugador ha aumentado " + settings.statName + " en " + roundedIncreaseAmount + "!");
        }
        else if (typeof(T) == typeof(float))
        {
            playerStat = (T)(object)((float)(object)playerStat + statIncreaseAmount);
            Debug.Log("El jugador ha aumentado " + settings.statName + " en " + statIncreaseAmount + "!");
        }
        else
        {
            throw new NotSupportedException("El tipo de dato no es compatible.");
        }
    }
}

    public void AddExperience(int experience)
    {
        Debug.Log("AddExperience succesfully called.");
        currentExperience += experience;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        Debug.Log("CheckLevelUp succesfully called.");
        while (currentLevel < maxLevel && currentExperience >= experienceRequiredPerLevel[currentLevel - 1])
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Debug.Log("LevelUp succesfully called.");
        currentExperience -= experienceRequiredPerLevel[currentLevel - 1];
        currentLevel++;
        HandleLevelUp(currentLevel);

        Debug.Log("¡" + characterName + " ha subido de nivel al nivel " + currentLevel + "!");
        // Activar las animaciones de Particle System
            particleSystem1.SetActive(true);
            particleSystem2.SetActive(true);
            particleSystem3.SetActive(true);

            // Obtener los componentes ParticleSystem de los GameObjects
            ParticleSystem ps1 = particleSystem1.GetComponent<ParticleSystem>();
            ParticleSystem ps2 = particleSystem2.GetComponent<ParticleSystem>();
            ParticleSystem ps3 = particleSystem3.GetComponent<ParticleSystem>();

            // Reproducir las animaciones de los sistemas de partículas
            ps1.Play();
            ps2.Play();
            ps3.Play();

            // Iniciar una corrutina para desactivar las animaciones después de cierto tiempo
            StartCoroutine(DeactivateParticleSystems());
    }

    private IEnumerator MoveLvlText(GameObject lvlTextObject, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = .8f;
        Vector3 initialPosition = lvlTextObject.transform.position;

        while (elapsedTime < duration && lvlTextObject != null) // Verificar si el objeto aún existe
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            lvlTextObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        // Destruir el objeto clonado después de 1 segundo
        if (lvlTextObject != null)
        {
            Destroy(lvlTextObject, .8f);
        }
    }

    private IEnumerator MoveEatMeatText(GameObject eatMeatTextObject, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = .8f;
        Vector3 initialPosition = eatMeatTextObject.transform.position;

        while (elapsedTime < duration && eatMeatTextObject != null) // Verificar si el objeto aún existe
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            eatMeatTextObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        // Destruir el objeto clonado después de 1 segundo
        if (eatMeatTextObject != null)
        {
            Destroy(eatMeatTextObject, .8f);
        }
    }

    private IEnumerator MoveRoarText(GameObject roarTextObject, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = .8f;
        Vector3 initialPosition = roarTextObject.transform.position;

        while (elapsedTime < duration && roarTextObject != null) // Verificar si el objeto aún existe
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            roarTextObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        // Destruir el objeto clonado después de 1 segundo
        if (roarTextObject != null)
        {
            Destroy(roarTextObject, .8f);
        }
    }

    private IEnumerator DeactivateParticleSystems()
    {
        yield return new WaitForSeconds(2f);

        // Verificar si el componente ParticleSystem está presente antes de desactivar las animaciones
        if (particleSystem1.TryGetComponent<ParticleSystem>(out ParticleSystem ps1))
        {
            ps1.Stop();
            particleSystem1.SetActive(false);
        }

        if (particleSystem2.TryGetComponent<ParticleSystem>(out ParticleSystem ps2))
        {
            ps2.Stop();
            particleSystem2.SetActive(false);
        }

        if (particleSystem3.TryGetComponent<ParticleSystem>(out ParticleSystem ps3))
        {
            ps3.Stop();
            particleSystem3.SetActive(false);
        }
    }
}
