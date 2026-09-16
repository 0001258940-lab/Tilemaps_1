using UnityEngine;
using UnityEngine.SceneManagement;

public enum TipoObstaculo { Serra, Laser }

public class GerenciadorArmadilhas : MonoBehaviour
{
    [Header("Configuração Geral")]
    [SerializeField] private TipoObstaculo tipo = TipoObstaculo.Serra;

    [Header("Configurações da Serra")]
    [SerializeField] private float distancia = 4f;
    [SerializeField] private float velocidade = 3f;

    [Header("Configurações do Laser")]
    [SerializeField] private float tempoAtivo = 2f;     // Quanto tempo fica ligado
    [SerializeField] private float tempoInativo = 1.5f; // Quanto tempo fica desligado
    [SerializeField] private float atrasoInicial = 0f;  // Atraso antes do 1º ciclo
    [SerializeField] private Collider2D colisorLaser;

    private Vector3 posInicial;
    private float cronometro;
    private bool emAtraso = false;
    private Animator anim;

    private void Start()
    {
        posInicial = transform.position;
        anim = GetComponent<Animator>();

        if (colisorLaser == null) colisorLaser = GetComponent<Collider2D>();

        if (tipo == TipoObstaculo.Laser && atrasoInicial > 0)
        {
            emAtraso = true;
            DesligarLaser();
        }
    }

    private void Update()
    {
        switch (tipo)
        {
            case TipoObstaculo.Serra:
                AtualizarSerra();
                break;

            case TipoObstaculo.Laser:
                AtualizarLaser();
                break;
        }
    }

    private void AtualizarSerra()
    {
        // Movimento de vai e vem automático sem necessidade de Corrotina
        float deslocamento = Mathf.PingPong(Time.time * velocidade, distancia);
        transform.position = posInicial + new Vector3(deslocamento, 0, 0);
    }

    private void AtualizarLaser()
    {
        // Lógica do Atraso Inicial
        if (emAtraso)
        {
            cronometro += Time.deltaTime;
            if (cronometro >= atrasoInicial)
            {
                emAtraso = false;
                cronometro = 0f;
            }
            return;
        }

        // Ciclo do Tempo Ativo e Inativo
        cronometro += Time.deltaTime;
        float cicloTotal = tempoAtivo + tempoInativo;

        if (cronometro < tempoAtivo)
        {
            LigarLaser();
        }
        else if (cronometro < cicloTotal)
        {
            DesligarLaser();
        }
        else
        {
            cronometro = 0f;
        }
    }

    private void LigarLaser()
    {
        if (colisorLaser != null) colisorLaser.enabled = true;
        if (anim != null) anim.SetBool("Ativado", true);
    }

    private void DesligarLaser()
    {
        if (colisorLaser != null) colisorLaser.enabled = false;
        if (anim != null) anim.SetBool("Ativado", false);
    }

    // --- DETECÇÃO DE DANO E REINÍCIO ---
    private void OnTriggerEnter2D(Collider2D collider)
    {
        ProcessarMorte(collider.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessarMorte(collision.gameObject);
    }

    private void ProcessarMorte(GameObject objetoAtingido)
    {
        if (objetoAtingido.CompareTag("Player"))
        {
            Destroy(objetoAtingido);
            Invoke(nameof(ReiniciarCena), 1.5f);
        }
    }

    private void ReiniciarCena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}