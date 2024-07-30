using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance { get; private set; }

    public GameObject loadingScreen;
    private Animator loadingScreenAnimator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        loadingScreen.SetActive(false);
        loadingScreenAnimator = loadingScreen.GetComponent<Animator>();
    }
    
    public void ShowLoadingScreen()
    {
        loadingScreen.SetActive(true);
        loadingScreenAnimator.Play("LogoExpand");
    }

    public void HideLoadingScreen()
    {
        loadingScreen.SetActive(false);
        Invoke("DeactivateLoadingScreen", 9f); // Asegúrate de que el tiempo aquí coincide con la duración de la animación.
    }

    private void DeactivateLoadingScreen()
    {
        loadingScreen.SetActive(false);
    }
}