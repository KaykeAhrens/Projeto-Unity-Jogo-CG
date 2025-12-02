using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SistemaGasolina : MonoBehaviour
{
    [Header("Referências")]
    public Transform airplane;
    public Transform cam;
    public GameObject combustivelPrefab; 
    public aviao scriptAviao; 
    
    [Header("UI da Gasolina")]
    public Image barraGasolina; 
    public TextMeshProUGUI textoPorcentagem; 
    public Color corCheia = Color.green;
    public Color corMedia = Color.yellow;
    public Color corBaixa = Color.red;
    
    [Header("Configurações de Gasolina")]
    public float gasolinaMaxima = 100f;
    public float gasolinaAtual = 100f;
    public float gastoPorSegundo = 8f;
    public float gastoExtraAoUsarD = 5f;
    
    [Header("Configurações de Combustível")]
    public float distanciaEntreCombustiveis = 25f;
    public float combustivelRecuperado = 60f;
    public float minY = 0.5f;
    public float maxY = 4f;
    public float distanciaAFrente = 15f;
    
    private float proximoCombustivelX = 0f;
    private bool jogoIniciado = false;
    
    void Start()
    {
        gasolinaAtual = gasolinaMaxima;
        
        if (airplane != null)
        {
            proximoCombustivelX = airplane.position.x + distanciaAFrente;
            SpawnarCombustivel();
        }
        
        jogoIniciado = true;
    }
    
    void Update()
    {
        if (!jogoIniciado || airplane == null) return;
        
        // Usa Time.unscaledDeltaTime se o jogo pausar com timeScale
        float deltaTime = Time.deltaTime;
        
        // ========== CONSUMO DE GASOLINA ==========
        Consumir(deltaTime);
        
        // ========== ATUALIZAR BARRA VISUAL ==========
        AtualizarBarraUI();
        
        // ========== SPAWNAR PRÓXIMO COMBUSTÍVEL ==========
        if (airplane.position.x > proximoCombustivelX - distanciaAFrente)
        {
            proximoCombustivelX = airplane.position.x + distanciaEntreCombustiveis;
            SpawnarCombustivel();
        }
        
        // ========== GASOLINA ZEROU! ==========
        if (gasolinaAtual <= 0)
        {
            FicouSemGasolina();
        }
    }
    
    void Consumir(float deltaTime)
    {
        // Gasto base constante
        gasolinaAtual -= gastoPorSegundo * deltaTime;
        
        // Gasto extra quando aperta D
        if (Input.GetKey(KeyCode.D))
        {
            gasolinaAtual -= gastoExtraAoUsarD * deltaTime;
        }
        
        gasolinaAtual = Mathf.Max(gasolinaAtual, 0);
    }
    
    void AtualizarBarraUI()
    {
        if (barraGasolina == null) return;
        
        float percentual = gasolinaAtual / gasolinaMaxima;
        barraGasolina.fillAmount = percentual;
        
        if (percentual > 0.5f)
        {
            float t = (percentual - 0.5f) / 0.5f;
            barraGasolina.color = Color.Lerp(corMedia, corCheia, t);
        }
        else
        {
            float t = percentual / 0.5f;
            barraGasolina.color = Color.Lerp(corBaixa, corMedia, t);
        }
        
        if (textoPorcentagem != null)
        {
            int porcentagemInt = Mathf.RoundToInt(percentual * 100);
            textoPorcentagem.text = porcentagemInt + "%";
        }
    }
    
    void SpawnarCombustivel()
    {
        if (combustivelPrefab == null) return;
        
        float y = Random.Range(minY, maxY);
        Vector3 pos = new Vector3(proximoCombustivelX, y, 0);
        
        GameObject c = Instantiate(combustivelPrefab, pos, Quaternion.identity);
        
        CombustivelItem item = c.AddComponent<CombustivelItem>();
        item.sistemaGasolina = this;
        item.airplane = airplane;
    }
    
    public void ColetarCombustivel()
    {
        gasolinaAtual += combustivelRecuperado;
        
        if (gasolinaAtual > gasolinaMaxima)
            gasolinaAtual = gasolinaMaxima;
        
        Debug.Log("⛽ Combustível coletado! Gasolina: " + gasolinaAtual);
    }
    
    void FicouSemGasolina()
    {
        Debug.Log("💀 FICOU SEM GASOLINA!");
        
        if (scriptAviao != null)
        {
            scriptAviao.PiscarAviao();
            scriptAviao.PerderVida();
        }
        
        gasolinaAtual = gasolinaMaxima * 0.4f;
    }
}

public class CombustivelItem : MonoBehaviour
{
    public SistemaGasolina sistemaGasolina;
    public Transform airplane;
    public float velocidadeRotacao = 100f;
    
    void Update()
    {
        // Rotação visual
        transform.Rotate(0, 0, velocidadeRotacao * Time.deltaTime);
        
        // Destrói quando passa do avião
        if (airplane != null && transform.position.x < airplane.position.x - 15f)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (sistemaGasolina != null)
            {
                sistemaGasolina.ColetarCombustivel();
            }
            
            Destroy(gameObject);
        }
    }
}