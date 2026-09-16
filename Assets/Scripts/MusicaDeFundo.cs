using UnityEngine;

public class MusicaFundo : MonoBehaviour
{
    // Variável estática que vai guardar 'este' objeto oficial da música
    private static MusicaFundo instancia;

    private void Awake()
    {
        // Se ainda não temos nenhuma música registrada...
        if (instancia == null)
        {
            instancia = this; // O 'this' registra ESTE objeto atual como o principal
            DontDestroyOnLoad(gameObject); // Impede que ESTE objeto seja destruído ao recarregar a cena
        }
        else
        {
            // Se a variável 'instancia' já possui o 'this' da música anterior, destrói esta cópia nova
            Destroy(gameObject);
        }
    }
}
