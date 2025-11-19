using UnityEngine;

public class BichoSpawner : MonoBehaviour
{
    public GameObject bichoPrefab;
    public Transform airplane;
    public Transform cam;
    
    [Header("Configurações de Spawn")]
    public float distanceAhead = 12f;
    public float minSpawnTime = 4f;
    public float maxSpawnTime = 8f;
    public float minY = -3f;
    public float maxY = 3f;
    
    private float nextSpawnTime = 0f;
    
    void Update()
    {
        if (cam == null || airplane == null) return;
        
        transform.position = new Vector3(
            cam.position.x + distanceAhead,
            0,
            0
        );
        
        if (Time.time >= nextSpawnTime)
        {
            Spawnbicho();
            nextSpawnTime = Time.time + Random.Range(minSpawnTime, maxSpawnTime);
        }
    }
    
    void Spawnbicho()
    {
        float y = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(
            transform.position.x,
            y,
            0
        );
        
        GameObject b = Instantiate(bichoPrefab, spawnPos, Quaternion.identity);
        bichoMovement bm = b.AddComponent<bichoMovement>();
        bm.speed = 2f;
        bm.airplane = airplane;
    }
}

public class bichoMovement : MonoBehaviour
{
    public float speed = 2f;
    public Transform airplane;
    
    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
        
        if (airplane != null && transform.position.x < airplane.position.x - 10f)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            aviao a = collision.GetComponent<aviao>();
            if (a != null)
            {
                a.PiscarAviao(); // FAZ PISCAR
                a.PerderVida();
            }
            Destroy(gameObject);
        }
    }
}