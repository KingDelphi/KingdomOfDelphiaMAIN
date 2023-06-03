using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject babySpawner;
    [SerializeField]
    private GameObject adultSpawner;

    [SerializeField]
    private float babyInterval = 3.5f;
    [SerializeField]
    private float adultInterval = 10f;

    [SerializeField]
    private Collider2D spawnArea;

    private int currentNumBabys = 0;
    private int currentNumAdults = 0;

    [SerializeField]
    private int maxNumBabys = 0;
    [SerializeField]
    private int maxNumAdults = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnBabys());
        StartCoroutine(SpawnAdults());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SpawnBabys()
    {
        while (true)
        {
            if (currentNumBabys < maxNumBabys)
            {
                SpawnBaby();
                currentNumBabys++;
            }
            yield return new WaitForSeconds(babyInterval);
        }
    }

    private IEnumerator SpawnAdults()
    {
        while (true)
        {
            if (currentNumAdults < maxNumAdults)
            {
                SpawnAdult();
                currentNumAdults++;
            }
            yield return new WaitForSeconds(adultInterval);
        }
    }

    private void SpawnBaby()
    {
        Vector2 randomPoint = RandomPointInPolygon(spawnArea as PolygonCollider2D);
        GameObject newEnemy = Instantiate(babySpawner, randomPoint, Quaternion.identity);

        // Suscribirse al evento de muerte del enemigo
        Monsters monster = newEnemy.GetComponent<Monsters>();
        if (monster != null)
        {
            monster.OnDeath += DecreaseNumBabys;
        }
    }

    private void SpawnAdult()
    {
        Vector2 randomPoint = RandomPointInPolygon(spawnArea as PolygonCollider2D);
        GameObject newEnemy = Instantiate(adultSpawner, randomPoint, Quaternion.identity);

        // Suscribirse al evento de muerte del enemigo
        Monsters monster = newEnemy.GetComponent<Monsters>();
        if (monster != null)
        {
            monster.OnDeath += DecreaseNumAdults;
        }
    }

    private void DecreaseNumBabys()
    {
        currentNumBabys--;
    }

    private void DecreaseNumAdults()
    {
        currentNumAdults--;
    }

    private Vector2 RandomPointInPolygon(PolygonCollider2D polygonCollider)
    {
        Bounds bounds = polygonCollider.bounds;
        int maxAttempts = 100;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPoint = new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));

            if (polygonCollider.OverlapPoint(randomPoint))
            {
                return randomPoint;
            }
        }

        Debug.LogWarning("No se pudo encontrar un punto dentro del polígono después de varios intentos.");

        // Si no se encontró un punto dentro del polígono, simplemente devuelve un punto aleatorio dentro del rectángulo delimitador
        return new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));
    }
}