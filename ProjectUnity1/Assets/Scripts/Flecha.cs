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
        if (collision.gameObject == emisor)
            return;

        if (collision.TryGetComponent<Sheep>(out _))
            return;

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
                // Inicia la corrutina para aplicar el daño tras un retardo
                StartCoroutine(AplicarDanioConRetardo(atacable));
                return;
            }
        }
    }

    private System.Collections.IEnumerator AplicarDanioConRetardo(IAtacable atacable)
    {
        yield return new WaitForSeconds(0.15f); // Ajusta el tiempo según la animación de impacto
        atacable.RecibirDanio(danio, gameObject);
        Destroy(gameObject);
    }
}
    