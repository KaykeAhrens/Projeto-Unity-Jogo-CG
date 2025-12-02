using TMPro;
using UnityEngine;
using System.Collections;

public class aviao : MonoBehaviour
{
    public float speed = 0.1f;
    public float speedUp = 3f;
    private Rigidbody2D rb;
    public int coins = 0;
    public TextMeshProUGUI coinsText;
    public Sprite[] sprites;
    private SpriteRenderer sr;
    public Camera cam;
    public int vidas = 3;
    public TextMeshProUGUI vidasText;
    
    [Header("Limites Verticais")]
    public float limiteInferior = -5.5f;  // Não pode descer mais que isso
    public float limiteSuperior = 4.5f; // Não pode subir mais que isso
    
    [Header("Fundo infinito")]
    public Transform[] fundos;  
    private float larguraFundo;   
    private int proximoFundo = 0;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        vidasText.text = "Vidas: " + vidas;
        
        if (fundos.Length > 0)
            larguraFundo = fundos[0].GetComponent<SpriteRenderer>().bounds.size.x;
    }
    
    void Update()
    {
        movimentacao();
        salto();
        camera();
        fundoInfinito();
        LimitarPosicao(); // NOVA FUNÇÃO!
        transform.position += Vector3.right * speed * 3f;
    }
    
    private void LimitarPosicao()
    {
        // Pega a posição atual
        Vector3 pos = transform.position;
        
        // Limita o Y (altura)
        pos.y = Mathf.Clamp(pos.y, limiteInferior, limiteSuperior);
        
        // Aplica a posição limitada
        transform.position = pos;
        
        // Se bateu no limite superior, zera a velocidade vertical do Rigidbody
        if (pos.y >= limiteSuperior && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
        
        // Se bateu no limite inferior, zera a velocidade vertical do Rigidbody
        if (pos.y <= limiteInferior && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
    }
    
    private void camera()
    {
        if (transform.position.x < cam.transform.position.x - 3.38)
        {
            transform.position = new Vector2(cam.transform.position.x - 3.38f, transform.position.y);
        }
        if (transform.position.x > cam.transform.position.x)
        {
            cam.transform.position = new Vector3(transform.position.x, cam.transform.position.y, cam.transform.position.z);
        }
    }
    
    private void salto()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * speedUp);
        }
    }
    
    private void movimentacao()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * speed;
            sr.flipX = false;
            sr.sprite = sprites[0];
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * (speed * 5);
            sr.flipX = true;
            sr.sprite = sprites[0];
        }
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.Space))
        {
            sr.sprite = sprites[0];
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * speed;
            sr.sprite = sprites[2];
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.down * speed;
            sr.sprite = sprites[1];
        }
    }
    
    private void fundoInfinito()
    {
        if (fundos.Length < 2) return;
        
        if (transform.position.x > fundos[proximoFundo].position.x + larguraFundo)
        {
            int fundoAtras = proximoFundo;
            proximoFundo = (proximoFundo + 1) % fundos.Length;
            fundos[fundoAtras].position = new Vector3(
                fundos[proximoFundo].position.x + larguraFundo,
                fundos[fundoAtras].position.y,
                fundos[fundoAtras].position.z
            );
        }
    }
    
    public GameOverManager gameOverManager;
    
    public void PerderVida()
    {
        vidas--;
        vidasText.text = "Vidas: " + vidas;
        if (vidas <= 0)
        {
            Debug.Log("GAME OVER");
            gameOverManager.MostrarGameOver();
            gameObject.SetActive(false);
        }
    }
    
    public void PiscarAviao()
    {
        StartCoroutine(Piscar());
    }
    
    private IEnumerator Piscar()
    {
        Color corOriginal = sr.color;
        
        for (int i = 0; i < 3; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            
            sr.color = corOriginal;
            yield return new WaitForSeconds(0.1f);
        }
    }
}