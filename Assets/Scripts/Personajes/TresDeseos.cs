using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class TresDeseos : MonoBehaviour
{
    public string characterName = "Tres Deseos";
    public int maxLevel = 99;
    public int unlockNextCharacterLevel = 99;

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

    private GameObject lvlTextObject;
    public GameObject lvlTextPrefab;

    public Animator animator;

    private float vomitTimer = 0f;
    private GameObject vomitTextObject;
    public GameObject vomitTextPrefab;

    private float zombieBreathTimer = 0f;
    private GameObject zombieBreathTextObject;
    public GameObject zombieBreathTextPrefab;

    [SerializeField]
    private GameObject particleSystem1;

    [SerializeField]
    private GameObject particleSystem2;

    [SerializeField]
    private GameObject particleSystem3;

    public int[] experienceRequiredPerLevel = {
        100, 200, 400, 800, 1500, 2600, 4200, 6400, 9300, 13000, 17600, 23200, 29900, 37800,
        47000, 57600, 69700, 83400, 98800, 116000, 135100, 156200, 179400, 204800, 232500, 262600, 295200, 330400, 368300,
        409000, 452600, 499200, 548900, 601800, 658000, 717600, 780700, 847400, 917800, 992000, 1070100, 1152200, 1238400, 1328800,
        1423500, 1522600, 1626200, 1734400, 1847300, 1965000, 2087600, 2215200, 2347900, 2485800, 2629000, 2777600, 2931700, 3091400, 3256800,
        3428000, 3605100, 3788200, 3977400, 4172800, 4374500, 4582600, 4797200, 5018400, 5246300, 5481000, 5722600, 5971200, 6226900, 6489800,
        6760000, 7037600, 7322700, 7615400, 7915800, 8224000, 8540100, 8864200, 9196400, 9536800, 9885500, 10242600, 10608200, 10982400, 11365300,
        11757000, 1217600, 12567200, 12985900, 13413800, 13851000, 14297600, 14753700, 15219400
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

        // Vomit
        if (Input.GetKeyDown(KeyCode.Alpha1) && vomitTimer <= 0)
        {
            Vomit();
        }

        // Actualizar el temporizador de cooldown de Vomit
        if (vomitTimer > 0f)
        {
            vomitTimer -= Time.deltaTime;

            if (vomitTimer <= 0f)
            {
                Debug.Log("El cooldown de Vomit ha finalizado.");
            }
        }

        // Zombie Breath
        if (Input.GetKeyDown(KeyCode.Alpha2) && zombieBreathTimer <= 0)
        {
            ZombieBreath();
        }

        // Actualizar el temporizador de cooldown de Zombie Breath
        if (zombieBreathTimer > 0f)
        {
            zombieBreathTimer -= Time.deltaTime;

            if (zombieBreathTimer <= 0f)
            {
                Debug.Log("El cooldown de Zombie Breath ha finalizado.");
            }
        }
    }

    private void Vomit()
    {
        // Obtener el componente Player del jugador
        Player player = GetComponent<Player>();
        PlayerAttack playerAttack = GetComponent<PlayerAttack>();

        if (player.currentMana >= 8 && playerAttack.selectedMonster != null)
        {
            player.ConsumeMana(8);
        }
        else
        {
            return;
        }

        if (vomitTimer > 0f)
        {
            Debug.Log("Vomit aún está en cooldown!");
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
                float distanceToEnemy = Vector3.Distance(playerAttack.selectedMonster.transform.position, enemy.transform.position);
                if (distanceToEnemy <= playerAttack.attackRange)
                {
                    Debug.Log("Enemigo seleccionado: " + enemy.name);
                    VomitAttack(enemy);
                }
            }
        }

        // Establecer el cooldown de Vomit a 5 segundos
        vomitTimer = 5f;

        Debug.Log("Vomit ha sido utilizado! Cerebrito vomita sobre sus enemigos.");

        StartCoroutine(ResetVomitCooldown());

        // Calcular la posición vertical del texto de nivel por encima del personaje
            Vector3 characterHeight = player.transform.localScale;
            float yOffset = characterHeight.y;
            Vector3 vomitTextPosition = player.transform.position + Vector3.up * yOffset + new Vector3(0f, 2.5f, 0f);

            // Instanciar el objeto de texto de nivel y almacenar la referencia en lvlTextObject
            vomitTextObject = Instantiate(vomitTextPrefab, vomitTextPosition, Quaternion.identity);

            // Obtener el componente TextMeshPro del objeto de texto de nivel
            TextMeshPro vomitText = vomitTextObject.GetComponent<TextMeshPro>();

            // Establecer el color del texto de nivel
            vomitText.color = Color.red;

            // Mostrar el nivel en el texto
            vomitText.text = ("¡Wacala!");

            // Iniciar la corutina para animar la posición vertical del texto de nivel durante 1 segundo
            StartCoroutine(MoveVomitText(vomitTextObject, vomitTextPosition + Vector3.up));
    }

    private void VomitAttack(Monsters enemy)
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

    private IEnumerator ResetVomitCooldown()
    {
        yield return new WaitForSeconds(vomitTimer);

        Debug.Log("El cooldown de Vomit ha finalizado.");
    }

    private void ZombieBreath()
    {
        // Obtener el componente Player del jugador
        Player player = GetComponent<Player>();
        PlayerAttack playerAttack = GetComponent<PlayerAttack>();

        if (player.currentMana >= 6 && playerAttack.selectedMonster != null)
        {
            player.ConsumeMana(6);
        }
        else
        {
            return;
        }

        if (zombieBreathTimer > 0f)
        {
            Debug.Log("Zombie Breath aún está en cooldown!");
            return;
        }

        // Obtener todos los objetos con la etiqueta "Monster" en la escena
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Realizar el ataque a cada jugador en el rango
        foreach (GameObject playerObject in players)
        {
            if (playerObject != null)
            {
                float distanceToEnemy = Vector3.Distance(playerAttack.selectedMonster.transform.position, playerObject.transform.position);
                if (distanceToEnemy <= playerAttack.attackRange)
                {
                    Debug.Log("Jugador seleccionado: " + player.name);
                    ZombieBreathAttack(playerObject.gameObject);
                }
            }
        }

        // Establecer el cooldown de Zombie Breath a 5 segundos
        zombieBreathTimer = 5f;

        Debug.Log("Zombie Breath ha sido utilizado! El olor de Cerebrito cura a sus aliados.");

        StartCoroutine(ResetZombieBreathCooldown());

        // Calcular la posición vertical del texto de nivel por encima del personaje
            Vector3 characterHeight = player.transform.localScale;
            float yOffset = characterHeight.y;
            Vector3 zombieBreathTextPosition = player.transform.position + Vector3.up * yOffset + new Vector3(0f, 2.5f, 0f);

            // Instanciar el objeto de texto de nivel y almacenar la referencia en lvlTextObject
            zombieBreathTextObject = Instantiate(zombieBreathTextPrefab, zombieBreathTextPosition, Quaternion.identity);

            // Obtener el componente TextMeshPro del objeto de texto de nivel
            TextMeshPro zombieBreathText = zombieBreathTextObject.GetComponent<TextMeshPro>();

            // Establecer el color del texto de nivel
            zombieBreathText.color = Color.green;

            // Mostrar el nivel en el texto
            zombieBreathText.text = ("Chicle de menta!");

            // Iniciar la corutina para animar la posición vertical del texto de nivel durante 1 segundo
            StartCoroutine(MoveZombieBreathText(zombieBreathTextObject, zombieBreathTextPosition + Vector3.up));
    }

    private void ZombieBreathAttack(GameObject playerObject)
    {
        Player player = playerObject.GetComponent<Player>();
        if (player != null)
        {
            player.Heal(8);
            
        }
        else
        {
            Debug.Log("El jugador es nulo. No se puede realizar el ataque.");
        }
    }

    private IEnumerator ResetZombieBreathCooldown()
    {
        yield return new WaitForSeconds(zombieBreathTimer);

        Debug.Log("El cooldown de ZombieBreath ha finalizado.");
    }

    private void HandleLevelUp(int level)
    {
        Debug.Log("HandleLevelUp succesfully called. DEBERIA MOSTRAR EL TEXTO DE QUE CEREBRITO SUBIO DE LVL!!!");
        if ((level > (currentLevel - 1)) && level <= maxLevel)
        {
            currentLevel = level;

            // Incrementar las estadísticas según las probabilidades y rangos de aumento
            IncreaseStats(ref player.hp, hpIncreaseSettings);
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

            player.currentHealth = player.hp;
            
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

    private IEnumerator MoveVomitText(GameObject vomitTextObject, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = .8f;
        Vector3 initialPosition = vomitTextObject.transform.position;

        while (elapsedTime < duration && vomitTextObject != null) // Verificar si el objeto aún existe
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            vomitTextObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        // Destruir el objeto clonado después de 1 segundo
        if (vomitTextObject != null)
        {
            Destroy(vomitTextObject, .8f);
        }
    }

    private IEnumerator MoveZombieBreathText(GameObject zombieBreathTextObject, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = .8f;
        Vector3 initialPosition = zombieBreathTextObject.transform.position;

        while (elapsedTime < duration && zombieBreathTextObject != null) // Verificar si el objeto aún existe
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            zombieBreathTextObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        // Destruir el objeto clonado después de 1 segundo
        if (zombieBreathTextObject != null)
        {
            Destroy(zombieBreathTextObject, .8f);
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