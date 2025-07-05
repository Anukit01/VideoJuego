using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GestorOrdenVisualGlobal : MonoBehaviour
{
    [Tooltip("Tag que deben tener los objetos que se van a ordenar.")]
    [SerializeField] private string tagFiltrado = "SortObject";

    [Tooltip("Ignorar objetos que contienen Canvas (UI).")]
    [SerializeField] private bool ignorarCanvas = true;

    void Start()
    {
        OrdenarTodoElMundo();
    }

    /// <summary>
    /// Reordena todos los objetos con el tag indicado, sin interferir con UI.
    /// </summary>
    public void OrdenarTodoElMundo()
    {
        GameObject[] objetos = GameObject.FindGameObjectsWithTag(tagFiltrado);
        List<(SpriteRenderer sr, float y)> lista = new();

        foreach (var obj in objetos)
        {
            SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>(true);

            foreach (var sr in renderers)
            {
                if (sr == null) continue;

                // Ignorar cualquier renderer que pertenezca a UI Canvas
                if (ignorarCanvas && sr.GetComponentInParent<Canvas>() != null)
                    continue;

                lista.Add((sr, sr.transform.position.y));
            }
        }

        var ordenados = lista.OrderBy(e => e.y).ToList();

        for (int i = 0; i < ordenados.Count; i++)
        {
            int orden = ordenados.Count - i; // Más abajo en Y = más adelante
            ordenados[i].sr.sortingOrder = orden;
        }

        Debug.Log($" Orden visual global completo: {ordenados.Count} objetos acomodados");
    }
}