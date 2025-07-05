using UnityEngine;

public class Flecha : MonoBehaviour
{
    private int danio;
    private GameObject emisor;
    private Vector2 puntoInicial;

    [SerializeField] private float velocidad = 10f;
    [SerializeField] private float distanciaMaxima = 12f;

    public void SetDanio(int cantidad) => danio = cantidad;
    public void SetEmisor(GameObject quienDisparo) => emisor = quienDisparo;

    void Start()
    {
        puntoInicial = transform.position;
        Destroy(gameObject, 4f); // Por seguridad, en caso de que no impacte
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, puntoInicial) >= distanciaMaxima)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // No impactar contra el emisor
        if (collision.gameObject == emisor)
            return;

        // Ignorar ovejas u objetos que no deben recibir daño
        if (collision.TryGetComponent<Sheep>(out _))
            return;

        // Verificar facción y aplicar daño si corresponde
        if (emisor.TryGetComponent<EntidadBase>(out var emisorEntidad) &&
            collision.TryGetComponent<EntidadBase>(out var objetivoEntidad))
        {
            if (!FaccionUtils.SonEnemigos(emisorEntidad.faccion, objetivoEntidad.faccion))
            {
                Debug.Log("No son enemigos. No se aplica daño.");
                return;
            }

            if (objetivoEntidad is IAtacable atacable)
            {
                Debug.Log($"La entidad es atacable. Aplicando daño...");
                atacable.RecibirDanio(danio);
                Destroy(gameObject);
                return;
            }
        }
    }
}