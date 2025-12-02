using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SistemaPontuacao : MonoBehaviour
{
    [Header("Referências")]
    public Transform airplane;
    public TextMeshProUGUI textoPontos; // Texto que aparece durante o jogo
    
    [Header("Configurações")]
    public float pontosMetro = 10f; // Quanto vale cada metro percorrido
    
    private float distanciaInicial;
    private float pontuacaoAtual = 0f;
    private bool jogoAtivo = true;
    
    void Start()
    {
        if (airplane != null)
        {
            distanciaInicial = airplane.position.x;
        }
    }
    
    void Update()
    {
        if (!jogoAtivo || airplane == null) return;
        
        // Calcula a distância percorrida
        float distanciaPercorrida = airplane.position.x - distanciaInicial;
        
        // Converte para pontos
        pontuacaoAtual = distanciaPercorrida * pontosMetro;
        
        // Atualiza o texto na tela
        if (textoPontos != null)
        {
            textoPontos.text = "PONTOS: " + Mathf.RoundToInt(pontuacaoAtual);
        }
    }
    
    public int ObterPontuacaoFinal()
    {
        return Mathf.RoundToInt(pontuacaoAtual);
    }
    
    public void PararContagem()
    {
        jogoAtivo = false;
    }
}