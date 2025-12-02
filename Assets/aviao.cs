using TMPro;
using UnityEngine;
using System.Collections;

public class aviao : MonoBehaviour
{
    [Header("Velocidades (unidades por segundo)")]
    public float speed = 5f;  // Velocidade base horizontal
    public float speedBoost = 25f;  // Velocidade ao apertar D
    public float speedVertical = 5f;  // Velocidade vertical (W/S)
    public float forcaPulo = 300f;  // Força do pulo (Space)
    
    private Rigidbody2D rb;
    public int coins = 0;
    public TextMeshProUGUI coinsText;
    public Sprite[] sprites;
    private SpriteRenderer sr;
    public Camera cam;
    public int vidas = 3;
    public TextMeshProUGUI vidasText;
    
    [Header("Limites Verticais")]
    public float limiteInferior = -5.5f;
    public float limiteSuperior = 4.5f;
    
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
        
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }
    
    void Update()
    {
        movimentacao();
        salto();
        camera();
        fundoInfinito();
        LimitarPosicao();
    }
    
    private void LimitarPosicao()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, limiteInferior, limiteSuperior);
        transform.position = pos;
        
        if (pos.y >= limiteSuperior && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
        
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
            rb.AddForce(Vector2.up * forcaPulo * Time.deltaTime);
        }
    }
    
    private void movimentacao()
    {
        // ========== MOVIMENTO HORIZONTAL ==========
        // SEMPRE se move para direita (velocidade base)
        float velocidadeAtual = speed;
        
        // Se apertar D, usa velocidade BOOST
        if (Input.GetKey(KeyCode.D))
        {
            velocidadeAtual = speedBoost;
            sr.flipX = true;
            sr.sprite = sprites[0];
        }
        // // Se apertar A, pode andar para trás (ou cancelar o movimento)
        // else if (Input.GetKey(KeyCode.A))
        // {
        //     velocidadeAtual = -speed; // Negativo = para trás
        //     sr.flipX = false;
        //     sr.sprite = sprites[0];
        // }
        
        // Aplica o movimento horizontal
        transform.position += Vector3.right * velocidadeAtual * Time.deltaTime;
        
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.Space))
        {
            sr.sprite = sprites[0];
        }
        
        // ========== MOVIMENTO VERTICAL ==========
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * speedVertical * Time.deltaTime;
            sr.sprite = sprites[2];
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.down * speedVertical * Time.deltaTime;
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
            if (gameOverManager != null)
            {
                gameOverManager.MostrarGameOver();
            }
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
            yield return new WaitForSecondsRealtime(0.1f);
            
            sr.color = corOriginal;
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}