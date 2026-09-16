using UnityEngine;

public class Dano : MonoBehaviour
{
    [Header("Configurações de Dano")]
    [Tooltip("Quantidade de dano que esta armadilha causará ao Player.")]
    public int valorDano = 1;

    // Detecta quando outro objeto entra na área de Trigger desta armadilha
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto colidido possui a Tag "Player"
        if (collision.CompareTag("Player"))
        {
            // Busca a classe/componente 'Vida' diretamente no Player colidido
            Vida scriptVida = collision.GetComponent<Vida>();

            // Se o script 'Vida' for encontrado no Player, executa o método de receber dano
            if (scriptVida != null)
            {
                scriptVida.ReceberDano(valorDano);
            }
            else
            {
                Debug.LogWarning($"[DANO] O objeto '{collision.name}' possui a tag Player, mas não contém o script 'Vida'!");
            }
        }
    }
}
