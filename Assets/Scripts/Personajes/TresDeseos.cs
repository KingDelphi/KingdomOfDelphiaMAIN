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

    private bool isFirstWishActive;
    private float firstWishEffectTimer;
    private GameObject firstWishTextObject;
    public GameObject firstWishTextPrefab;

    private bool isSecondWishActive;
    private float secondWishEffectTimer;
    private GameObject secondWishTextObject;
    public GameObject secondWishTextPrefab;

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
            // Debug.Log("La experiencia actual del jugador es: " + playerExperience);
        }

        // Cast "First Wish"
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isFirstWishActive)
        {
            FirstWish();
        }

        // Actualizar el temporizador de cooldown de Bad Words
        if (isFirstWishActive)
        {
            firstWishEffectTimer -= Time.deltaTime;

            if (firstWishEffectTimer <= 0f)
            {
                // Restablecer el cooldown de PlayerAttack
                PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
                playerAttack.attackCooldown = 2f;

                // Reiniciar el temporizador de cooldown
                firstWishEffectTimer = 0f;
                isFirstWishActive = false;

                Debug.Log("El cooldown de First Wish ha finalizado.");
            }
        }

        // Cast "Second Wish"
        if (Input.GetKeyDown(KeyCode.Alpha2) && !isSecondWishActive)
        {
            SecondWish();
        }

        // Actualizar el temporizador de cooldown de Bad Words
        if (isSecondWishActive)
        {
            secondWishEffectTimer -= Time.deltaTime;

            if (secondWishEffectTimer <= 0f)
            {
                // Restablecer el cooldown de PlayerAttack
                PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
                playerAttack.attackCooldown = 2f;

                // Reiniciar el temporizador de cooldown
                secondWishEffectTimer = 0f;
                isSecondWishActive = false;

                Debug.Log("El cooldown de Second Wish ha finalizado.");
            }
        }
    }

    private void FirstWish()
    {
        if (player.currentMana >= 12)
        {
            player.ConsumeMana(12);
        }
        else
        {
            return;
        }

        // Verificar si el cooldown de Bad Words está activo
        if (isFirstWishActive)
        {
            Debug.Log("¡First Wish aún está en cooldown!");
            return;
        }
        
        // Obtener el componente PlayerAttack del jugador
        PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();

        // Reducir el cooldown de PlayerAttack a 1.5f por 6 segundos
        playerAttack.attackCooldown = 1.5f;
        firstWishEffectTimer = 6f;

        // Activar el flag de Bad Words activo
        isFirstWishActive = true;

        StartCoroutine(ResetFirstWishCooldown(firstWishEffectTimer));

        Debug.Log("¡First Wish ha sido lanzado! El ataque se ha incrementado por 6 segundos.");

        // Calcular la posición vertical del texto de nivel por encima del personaje
            Vector3 characterHeight = player.transform.localScale;
            float yOffset = characterHeight.y;
            Vector3 firstWishTextPosition = player.transform.position + Vector3.up * yOffset + new Vector3(0f, 2.5f, 0f);

            // Instanciar el objeto de texto de nivel y almacenar la referencia en lvlTextObject
            firstWishTextObject = Instantiate(firstWishTextPrefab, firstWishTextPosition, Quaternion.identity);

            // Obtener el componente TextMeshPro del objeto de texto de nivel
            TextMeshPro firstWishText = firstWishTextObject.GetComponent<TextMeshPro>();

            // Establecer el color del texto de nivel
            firstWishText.color = Color.red;

            // Mostrar el nivel en el texto
            firstWishText.text = ("¡Deseo ATK!");

            // Iniciar la corutina para animar la posición vertical del texto de nivel durante 1 segundo
            StartCoroutine(MoveFirstWishText(firstWishTextObject, firstWishTextPosition + Vector3.up));
    }

    private IEnumerator ResetFirstWishCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        // Restablecer el cooldown de Bad Words
        isFirstWishActive = false;

        Debug.Log("El cooldown de First Wish ha finalizado.");
    }

    private void SecondWish()
    {
        if (player.currentMana >= 16)
        {
            player.ConsumeMana(16);
        }
        else
        {
            return;
        }

        // Verificar si el cooldown de Bad Words está activo
        if (isSecondWishActive)
        {
            Debug.Log("¡Second Wish aún está en cooldown!");
            return;
        }
        
        // Obtener el componente PlayerAttack del jugador
        PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();

        // Reducir el cooldown de PlayerAttack a 1.5f por 6 segundos
        playerAttack.attackCooldown = 1.5f;
        secondWishEffectTimer = 6f;

        // Activar el flag de Bad Words activo
        isSecondWishActive = true;

        StartCoroutine(ResetSecondWishCooldown(secondWishEffectTimer));

        Debug.Log("¡Second Wish ha sido lanzado! El cooldown de PlayerAttack se ha reducido a 1.5f por 6 segundos.");

        // Calcular la posición vertical del texto de nivel por encima del personaje
            Vector3 characterHeight = player.transform.localScale;
            float yOffset = characterHeight.y;
            Vector3 secondWishTextPosition = player.transform.position + Vector3.up * yOffset + new Vector3(0f, 2.5f, 0f);

            // Instanciar el objeto de texto de nivel y almacenar la referencia en lvlTextObject
            secondWishTextObject = Instantiate(secondWishTextPrefab, secondWishTextPosition, Quaternion.identity);

            // Obtener el componente TextMeshPro del objeto de texto de nivel
            TextMeshPro secondWishText = secondWishTextObject.GetComponent<TextMeshPro>();

            // Establecer el color del texto de nivel
            secondWishText.color = Color.red;

            // Mostrar el nivel en el texto
            secondWishText.text = ("¡Deseo ATK SPD!");

            // Iniciar la corutina para animar la posición vertical del texto de nivel durante 1 segundo
            StartCoroutine(MoveSecondWishText(secondWishTextObject, secondWishTextPosition + Vector3.up));
    }

    private IEnumerator ResetSecondWishCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        // Restablecer el cooldown de Bad Words
        isSecondWishActive = false;

        Debug.Log("El cooldown de Second Wish ha finalizado.");
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

    private IEnumerator MoveFirstWishText(GameObject firstWishTextObject, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = .8f;
        Vector3 initialPosition = firstWishTextObject.transform.position;

        while (elapsedTime < duration && firstWishTextObject != null) // Verificar si el objeto aún existe
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            firstWishTextObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        // Destruir el objeto clonado después de 1 segundo
        if (firstWishTextObject != null)
        {
            Destroy(firstWishTextObject, .8f);
        }
    }

    private IEnumerator MoveSecondWishText(GameObject secondWishTextObject, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = .8f;
        Vector3 initialPosition = secondWishTextObject.transform.position;

        while (elapsedTime < duration && secondWishTextObject != null) // Verificar si el objeto aún existe
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            secondWishTextObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        // Destruir el objeto clonado después de 1 segundo
        if (secondWishTextObject != null)
        {
            Destroy(secondWishTextObject, .8f);
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