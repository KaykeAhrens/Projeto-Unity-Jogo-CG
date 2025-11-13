using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class aviao : MonoBehaviour
{
    public float speed = 0.01f;
    public float speedUp = 3f;
    private Rigidbody2D rb;
    public int coins = 0;
    public TextMeshProUGUI coinsText;
    public Sprite[] sprites;
    private SpriteRenderer sr;
    public Camera cam;

    // --- Fundo infinito ---
    [Header("Fundo infinito")]
    public Transform[] fundos;  
    private float larguraFundo;   
    private int proximoFundo = 0; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // Calcula a largura do sprite do fundo automaticamente
        if (fundos.Length > 0)
            larguraFundo = fundos[0].GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        movimentacao();
        salto();
        camera();
        fundoInfinito();
        transform.position += Vector3.right * speed;
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

    // --- FUN��O DO FUNDO INFINITO ---
    private void fundoInfinito()
    {
        if (fundos.Length < 2) return;

        // Se acabou o fundo
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
}
