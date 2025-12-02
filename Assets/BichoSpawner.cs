using UnityEngine;

public class BichoSpawner : MonoBehaviour
{
    public GameObject bichoPrefab;
    public Transform airplane;
    public Transform cam;
    
    [Header("Configurações de Spawn")]
    public float distanceAhead = 12f;
    public float minSpawnTime = 2f;  // Mais rápido agora!
    public float maxSpawnTime = 5f;  // Mais rápido agora!
    public float minY = 0.5f;
    public float maxY = 4f;
    
    [Header("Múltiplos Pássaros")]
    public int minPassaros = 1;      // Mínimo de pássaros por vez
    public int maxPassaros = 4;      // Máximo de pássaros por vez
    public float espacamentoEntrePassaros = 1.5f; // Distância entre eles
    
    [Header("Movimento Circular")]
    [Range(0f, 1f)]
    public float chanceMovimentoCircular = 0.3f; // 30% de chance de circular
    
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
            SpawnarGrupoDePassaros();
            nextSpawnTime = Time.time + Random.Range(minSpawnTime, maxSpawnTime);
        }
    }
    
    void SpawnarGrupoDePassaros()
    {
        // Decide quantos pássaros vão aparecer desta vez
        int quantidadePassaros = Random.Range(minPassaros, maxPassaros + 1);
        
        for (int i = 0; i < quantidadePassaros; i++)
        {
            // Posição Y aleatória para cada pássaro
            float y = Random.Range(minY, maxY);
            
            // Espaça os pássaros horizontalmente
            float offsetX = i * espacamentoEntrePassaros;
            
            Vector3 spawnPos = new Vector3(
                transform.position.x + offsetX,
                y,
                0
            );
            
            GameObject b = Instantiate(bichoPrefab, spawnPos, Quaternion.identity);
            
            // Decide se este pássaro vai ter movimento circular
            bool vaiSerCircular = Random.value < chanceMovimentoCircular;
            
            if (vaiSerCircular)
            {
                // Adiciona movimento CIRCULAR
                bichoMovementCircular bmc = b.AddComponent<bichoMovementCircular>();
                bmc.speed = Random.Range(1.5f, 2.5f);
                bmc.raioCirculo = Random.Range(1f, 2f);
                bmc.velocidadeCirculo = Random.Range(2f, 4f);
                bmc.airplane = airplane;
            }
            else
            {
                // Adiciona movimento RETO normal
                bichoMovement bm = b.AddComponent<bichoMovement>();
                bm.speed = Random.Range(1.5f, 3f);
                bm.airplane = airplane;
            }
        }
    }
}

// ========== MOVIMENTO RETO (O ANTIGO) ==========
public class bichoMovement : MonoBehaviour
{
    public float speed = 2f;
    public Transform airplane;
    
    void Update()
    {
        // Move para a esquerda
        transform.position += Vector3.left * speed * Time.deltaTime;
        
        // Destrói quando passa do avião
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
                a.PiscarAviao();
                a.PerderVida();
            }
            Destroy(gameObject);
        }
    }
}

// ========== MOVIMENTO CIRCULAR (NOVO!) ==========
public class bichoMovementCircular : MonoBehaviour
{
    public float speed = 2f;           // Velocidade para a esquerda
    public float raioCirculo = 1.5f;   // Tamanho do círculo
    public float velocidadeCirculo = 3f; // Velocidade da rotação
    public Transform airplane;
    
    private Vector3 posicaoInicial;
    private float angulo = 0f;
    
    void Start()
    {
        posicaoInicial = transform.position;
    }
    
    void Update()
    {
        // Move para a esquerda
        posicaoInicial += Vector3.left * speed * Time.deltaTime;
        
        // Faz o movimento circular
        angulo += velocidadeCirculo * Time.deltaTime;
        
        float x = posicaoInicial.x + Mathf.Cos(angulo) * raioCirculo;
        float y = posicaoInicial.y + Mathf.Sin(angulo) * raioCirculo;
        
        transform.position = new Vector3(x, y, 0);
        
        // Rotaciona o sprite para parecer que está voando em círculo
        transform.rotation = Quaternion.Euler(0, 0, angulo * Mathf.Rad2Deg);
        
        // Destrói quando passa do avião
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
                a.PiscarAviao();
                a.PerderVida();
            }
            Destroy(gameObject);
        }
    }
}