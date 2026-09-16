using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Vida : MonoBehaviour
{
    [Header("Configurações de Vida")]
    [SerializeField] private int vidaMaxima = 100;
    private int vidaAtual; // Mantida privada para evitar sobrescrita incorreta no Inspector

    [Header("Feedback Visual de Dano (Flash)")]
    public SpriteRenderer spriteRenderer;
    public Color corDano = Color.red;       // Cor aplicada ao tomar dano
    public float duracaoFlash = 0.15f;      // Tempo (em segundos) que a cor vermelha dura

    private Color corOriginal = Color.white; // Armazena a cor padrão do sprite
    private float timerFlash;                // Contador de tempo manual
    private bool piscando;                   // Flag para controlar se está no estado de flash

    private void Awake()
    {
        // Se o SpriteRenderer não for arrastado no Inspector, busca automaticamente no objeto
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Armazena a cor original do Sprite no início do jogo
        if (spriteRenderer != null)
        {
            corOriginal = spriteRenderer.color;
        }
    }

    private void Start()
    {
        vidaAtual = vidaMaxima;
    }

    private void Update()
    {
        // --- GERENCIADOR MANUAL DO TEMPO DE FLASH ---
        if (piscando)
        {
            timerFlash += Time.deltaTime; // Soma o tempo de cada frame

            // Quando o tempo acumulado atinge ou passa da duração desejada:
            if (timerFlash >= duracaoFlash)
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = corOriginal; // Restaura a cor normal
                }
                piscando = false; // Desliga o estado de pisca
            }
        }
    }

    public void ReceberDano(int quantidadeDano)
    {
        // Trava para evitar processar dano se o jogador já estiver morto ou em invulnerabilidade temporária
        if (vidaAtual <= 0 || quantidadeDano <= 0) return;
        if (piscando) return;

        vidaAtual -= quantidadeDano;

        if (vidaAtual > 0)
        {
            // Aplica o efeito visual do Flash
            if (spriteRenderer != null)
            {
                spriteRenderer.color = corDano; // Muda a cor para Vermelho imediatamente
                timerFlash = 0f;               // Zera o temporizador
                piscando = true;               // Ativa o contador no Update
            }
        }
        else
        {
            vidaAtual = 0;
            Morrer();
        }
    }

    private void Morrer()
    {
        Debug.Log($"{gameObject.name} morreu!");
        SceneManager.LoadScene("GamePlay");
    }
}