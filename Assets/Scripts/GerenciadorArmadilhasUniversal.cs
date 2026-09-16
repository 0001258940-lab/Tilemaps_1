using UnityEngine;

// 1. Enum com os tipos exigidos
public enum TipoArmadilha { Espetos, Torreta }

public class ArmadilhasConfig : MonoBehaviour
{
    [Header("Tipo de Armadilha")]
    [SerializeField] private TipoArmadilha tipoArmadilha = TipoArmadilha.Espetos;

    [Header("Configurações Gerais")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D colisorDano;
    [SerializeField] private float atrasoInicial = 0f;

    [Header("Configurações dos Espetos")]
    [SerializeField] private float tempoAtivo = 2f;    // Tempo exposto (Estado 2)
    [SerializeField] private float tempoInativo = 2f;  // Tempo recolhido (Estado 3)

    [Header("Configurações da Torreta")]
    [SerializeField] private float tempoEntreDisparos = 3f;
    [SerializeField] private GameObject prefabProjetil;
    [SerializeField] private Transform pontoDisparo;
    [SerializeField] private float velocidadeProjetil = 10f;

    // Controle Interno de Tempos e Estados
    private float cronometro = 0f;
    private bool aguardandoAtraso = true;
    private int estadoEspeto = 1; // 1: Subindo, 2: Exposto, 3: Recolhendo

    private void Start()
    {
        // Auto-busca componentes caso não tenham sido atribuídos no Inspector
        if (animator == null) animator = GetComponent<Animator>();
        if (colisorDano == null) colisorDano = GetComponent<Collider2D>();
    }

    private void Update()
    {
        // Tratamento do Atraso Inicial compartilhado por ambas as armadilhas
        if (aguardandoAtraso)
        {
            cronometro += Time.deltaTime;
            if (cronometro >= atrasoInicial)
            {
                aguardandoAtraso = false;
                cronometro = 0f;
            }
            return; // Aguarda o atraso inicial terminar antes de rodar a lógica
        }

        // Seleção do comportamento via switch...case
        switch (tipoArmadilha)
        {
            case TipoArmadilha.Espetos:
                AtualizarEspetos();
                break;

            case TipoArmadilha.Torreta:
                AtualizarTorreta();
                break;
        }
    }

    #region Lógica dos Espetos
    private void AtualizarEspetos()
    {
        cronometro += Time.deltaTime;

        switch (estadoEspeto)
        {
            case 1: // Subindo
                DefinirEstadoAnimacao(1);
                if (colisorDano != null) colisorDano.enabled = true;
                
                // Transição rápida para a permanência exposta
                estadoEspeto = 2;
                cronometro = 0f;
                break;

            case 2: // Exposto (Dano ativo)
                DefinirEstadoAnimacao(2);
                if (colisorDano != null) colisorDano.enabled = true;

                if (cronometro >= tempoAtivo)
                {
                    estadoEspeto = 3;
                    cronometro = 0f;
                }
                break;

            case 3: // Recolhendo (Sem dano)
                DefinirEstadoAnimacao(3);
                if (colisorDano != null) colisorDano.enabled = false;

                if (cronometro >= tempoInativo)
                {
                    estadoEspeto = 1; // Reinicia o ciclo
                    cronometro = 0f;
                }
                break;
        }
    }
    #endregion

    #region Lógica da Torreta
    private void AtualizarTorreta()
    {
        cronometro += Time.deltaTime;

        if (cronometro >= tempoEntreDisparos)
        {
            DispararTorreta();
            cronometro = 0f;
        }
        else
        {
            // Estado 0: Idle (Aguardando o próximo disparo)
            DefinirEstadoAnimacao(0);
        }
    }

    private void DispararTorreta()
    {
        // Estado 1: Atirar
        DefinirEstadoAnimacao(1);

        if (prefabProjetil != null && pontoDisparo != null)
        {
            // Instancia o projétil na posição e rotação do pontoDisparo
            GameObject projetil = Instantiate(prefabProjetil, pontoDisparo.position, pontoDisparo.rotation);
            
            // Aplica velocidade no Rigidbody2D do projétil
            Rigidbody2D rb = projetil.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = pontoDisparo.right * velocidadeProjetil;
            }
        }
    }
    #endregion

    /// <summary>
    /// Envia o parâmetro int 'estado' exigido para o Animator.
    /// </summary>
    private void DefinirEstadoAnimacao(int estado)
    {
        if (animator != null)
        {
            animator.SetInteger("estado", estado);
        }
    }
}