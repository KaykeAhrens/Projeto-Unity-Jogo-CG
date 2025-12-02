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
    public float gastoPorSegundo = 8f; // Gasta 8 por segundo = dura ~12 segundos
    public float gastoExtraAoUsarD = 5f; // Gasta mais 5 quando aperta D
    
    [Header("Configurações de Combustível")]
    public float distanciaEntreCombustiveis = 25f; // Distância entre as latas
    public float combustivelRecuperado = 60f; // Quanto recupera ao pegar
    public float minY = 0.5f;
    public float maxY = 4f;
    public float distanciaAFrente = 15f; // Onde spawna na frente
    
    private float proximoCombustivelX = 0f;
    private bool jogoIniciado = false;
    
    void Start()
    {
        gasolinaAtual = gasolinaMaxima;
        
        if (airplane != null)
        {
            // Spawna o primeiro combustível um pouco à frente
            proximoCombustivelX = airplane.position.x + distanciaAFrente;
            SpawnarCombustivel();
        }
        
        jogoIniciado = true;
    }
    
    void Update()
    {
        if (!jogoIniciado || airplane == null) return;
        
        // ========== CONSUMO DE GASOLINA ==========
        Consumir();
        
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
    
    void Consumir()
    {
        // Gasto base constante
        gasolinaAtual -= gastoPorSegundo * Time.deltaTime;
        
        // Gasto extra quando aperta D
        if (Input.GetKey(KeyCode.D))
        {
            gasolinaAtual -= gastoExtraAoUsarD * Time.deltaTime;
        }
        
        // Não deixa ficar negativo
        gasolinaAtual = Mathf.Max(gasolinaAtual, 0);
    }
    
    void AtualizarBarraUI()
    {
        if (barraGasolina == null) return;
        
        // Atualiza o tamanho da barra
        float percentual = gasolinaAtual / gasolinaMaxima;
        barraGasolina.fillAmount = percentual;
        
        // Muda a cor de forma GRADUAL (verde → amarelo → vermelho)
        if (percentual > 0.5f)
        {
            // De 100% até 50%: Verde → Amarelo
            float t = (percentual - 0.5f) / 0.5f; // vai de 0 a 1
            barraGasolina.color = Color.Lerp(corMedia, corCheia, t);
        }
        else
        {
            // De 50% até 0%: Amarelo → Vermelho
            float t = percentual / 0.5f; // vai de 0 a 1
            barraGasolina.color = Color.Lerp(corBaixa, corMedia, t);
        }
        
        // Atualiza o texto da porcentagem (se existir)
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
        
        // Adiciona o script de coleta
        CombustivelItem item = c.AddComponent<CombustivelItem>();
        item.sistemaGasolina = this;
        item.airplane = airplane;
    }
    
    public void ColetarCombustivel()
    {
        gasolinaAtual += combustivelRecuperado;
        
        // Não deixa passar do máximo
        if (gasolinaAtual > gasolinaMaxima)
            gasolinaAtual = gasolinaMaxima;
        
        Debug.Log("⛽ Combustível coletado! Gasolina: " + gasolinaAtual);
    }
    
    void FicouSemGasolina()
    {
        Debug.Log("💀 FICOU SEM GASOLINA!");
        
        // Perde vida
        if (scriptAviao != null)
        {
            scriptAviao.PiscarAviao();
            scriptAviao.PerderVida();
        }
        
        // Dá um pouco de gasolina de volta pra não ficar travado
        gasolinaAtual = gasolinaMaxima * 0.4f; // Volta com 40%
    }
}

// ========== SCRIPT DO ITEM DE COMBUSTÍVEL ==========
public class CombustivelItem : MonoBehaviour
{
    public SistemaGasolina sistemaGasolina;
    public Transform airplane;
    public float velocidadeRotacao = 100f; // Faz girar bonitinho
    
    void Update()
    {
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
            // Coleta o combustível
            if (sistemaGasolina != null)
            {
                sistemaGasolina.ColetarCombustivel();
            }
            
            // Destrói a lata
            Destroy(gameObject);
        }
    }
}