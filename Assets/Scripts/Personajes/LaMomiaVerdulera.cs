using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class LaMomiaVerdulera : MonoBehaviour
{
    public string characterName = "La Momia Verdulera";
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

    private GameObject lvlTextObject;
    public GameObject lvlTextPrefab;

    public Animator animator;

    private bool isBadWordsActive;
    private float badWordsEffectTimer;
    private GameObject badWordTextObject;
    public GameObject badWordTextPrefab;


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

    public int GetLevel()
    {
        return currentLevel;
    }

    private void UpdateLevel()
    {
        player.SetLevel(GetLevel());
    }

    private void Start()
    {
        player = FindObjectOfType<Player>(); // Busca el objeto con el script "Player"
        UpdateLevel();

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
        }

        // Cast "Bad Words"
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isBadWordsActive)
        {
            BadWords();
        }

        // Actualizar el temporizador de cooldown de Bad Words
        if (isBadWordsActive)
        {
            badWordsEffectTimer -= Time.deltaTime;

            if (badWordsEffectTimer <= 0f)
            {
                // Restablecer el cooldown de PlayerAttack
                PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
                playerAttack.attackCooldown = 2f;

                // Reiniciar el temporizador de cooldown
                badWordsEffectTimer = 0f;
                isBadWordsActive = false;

                Debug.Log("El cooldown de Bad Words ha finalizado.");
            }
        }
    }

    private void BadWords()
    {
        if (player.currentMana >= 4)
        {
            player.ConsumeMana(4);
        }
        else
        {
            return;
        }

        // Verificar si el cooldown de Bad Words está activo
        if (isBadWordsActive)
        {
            Debug.Log("¡Bad Words aún está en cooldown!");
            return;
        }
        
        // Obtener el componente PlayerAttack del jugador
        PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();

        // Reducir el cooldown de PlayerAttack a 1.5f por 6 segundos
        playerAttack.attackCooldown = 1.5f;
        badWordsEffectTimer = 6f;

        // Activar el flag de Bad Words activo
        isBadWordsActive = true;

        StartCoroutine(ResetBadWordsCooldown(badWordsEffectTimer));

        Debug.Log("¡Bad Words ha sido lanzado! El cooldown de PlayerAttack se ha reducido a 1.5f por 6 segundos.");

        // Calcular la posición vertical del texto de nivel por encima del personaje
            Vector3 characterHeight = player.transform.localScale;
            float yOffset = characterHeight.y;
            Vector3 badWordTextPosition = player.transform.position + Vector3.up * yOffset + new Vector3(0f, 2.5f, 0f);

            // Instanciar el objeto de texto de nivel y almacenar la referencia en lvlTextObject
            badWordTextObject = Instantiate(badWordTextPrefab, badWordTextPosition, Quaternion.identity);

            // Obtener el componente TextMeshPro del objeto de texto de nivel
            TextMeshPro badWordText = badWordTextObject.GetComponent<TextMeshPro>();

            // Establecer el color del texto de nivel
            badWordText.color = Color.red;

            // Mostrar el nivel en el texto
            badWordText.text = ("@#$%&");

            // Iniciar la corutina para animar la posición vertical del texto de nivel durante 1 segundo
            StartCoroutine(MoveBadWordText(badWordTextObject, badWordTextPosition + Vector3.up));
    }

    private IEnumerator ResetBadWordsCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        // Restablecer el cooldown de Bad Words
        isBadWordsActive = false;

        Debug.Log("El cooldown de Bad Words ha finalizado.");
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

            UpdateLevel();
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

    private IEnumerator MoveBadWordText(GameObject badWordTextObject, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = .8f;
        Vector3 initialPosition = badWordTextObject.transform.position;

        while (elapsedTime < duration && badWordTextObject != null) // Verificar si el objeto aún existe
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            badWordTextObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        // Destruir el objeto clonado después de 1 segundo
        if (badWordTextObject != null)
        {
            Destroy(badWordTextObject, .8f);
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

[System.Serializable]
public class StatIncreaseSettings<T>
{
    public string statName;
    public float probability = 0.5f;
    public float minIncreaseAmount = 1;
    public float maxIncreaseAmount = 3;
}

[System.Serializable]
public class StatIncreaseSettingsInt : StatIncreaseSettings<int> { }

[System.Serializable]
public class StatIncreaseSettingsFloat : StatIncreaseSettings<float> { }
