using UnityEngine;
using System.Linq;

public class SortLayer : MonoBehaviour
{
    [SerializeField] private int offsetDelante = +1;
    [SerializeField] private int offsetDetras = -1;
    [SerializeField] private float margin = 0.5f;

    private SpriteRenderer[] propios;

    void Awake()
    {
        // Cacheamos todos los SpriteRenderer de este objeto y sus hijos
        propios = GetComponentsInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!EsObjetoOrdenable(other)) return;

        GameObject raiz = ObtenerRaizOrdenable(other);
        if (raiz == null) return;

        SpriteRenderer srReferencia = raiz.GetComponentsInChildren<SpriteRenderer>(true)
            .OrderBy(sr => sr.transform.position.y)
            .FirstOrDefault();

        if (srReferencia == null || propios.Length == 0) return;

        float yReferencia = ObtenerAlturaReferencia(raiz);
        float yUnidad = transform.position.y + margin;

        int ordenBase = srReferencia.sortingOrder;
        int nuevoOrden = yUnidad < yReferencia
            ? ordenBase + offsetDelante
            : ordenBase + offsetDetras;

        foreach (var sr in propios)
            sr.sortingOrder = nuevoOrden;
    }

    private bool EsObjetoOrdenable(Collider2D col)
    {
        Transform actual = col.transform;
        while (actual != null)
        {
            if (actual.CompareTag("SortObject"))
                return true;
            actual = actual.parent;
        }
        return false;
    }

    private GameObject ObtenerRaizOrdenable(Collider2D col)
    {
        Transform actual = col.transform;
        while (actual != null)
        {
            if (actual.CompareTag("SortObject"))
                return actual.gameObject;
            actual = actual.parent;
        }
        return null;
    }

    private float ObtenerAlturaReferencia(GameObject objeto)
    {
        BoxCollider2D col = objeto.GetComponentInChildren<BoxCollider2D>();
        return col != null ? col.transform.position.y : objeto.transform.position.y;
    }
}