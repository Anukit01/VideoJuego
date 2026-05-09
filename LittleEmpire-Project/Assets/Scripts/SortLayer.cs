using UnityEngine;
using System.Linq;

public class SortLayer : MonoBehaviour
{
    [SerializeField] private int offsetDelante = +10; // Usa un offset mayor para evitar conflictos
    [SerializeField] private int offsetDetras = -10;
    [SerializeField] private float margin = 0.5f;

    private SpriteRenderer[] propios;
    private int[] ordenOriginal; // Guarda el orden original para restaurar

    void Awake()
    {
        // Incluye el SpriteRenderer del objeto raíz si existe
        var propiosLista = GetComponentsInChildren<SpriteRenderer>(true).ToList();
        var srRaiz = GetComponent<SpriteRenderer>();
        if (srRaiz != null && !propiosLista.Contains(srRaiz))
            propiosLista.Add(srRaiz);

        propios = propiosLista.ToArray();
        ordenOriginal = propios.Select(sr => sr.sortingOrder).ToArray();
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

        for (int i = 0; i < propios.Length; i++)
            propios[i].sortingOrder = nuevoOrden + i; // Si hay varios renderers, mantener el orden relativo
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Al salir del trigger, restaurar el orden original
        for (int i = 0; i < propios.Length; i++)
            propios[i].sortingOrder = ordenOriginal[i];
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
