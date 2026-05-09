using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GestorOrdenVisualCamara : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tag que deben tener los objetos que se van a ordenar.")]
    [SerializeField] private string tagFiltrado = "SortObject";

    [Tooltip("Intervalo de actualización en segundos.")]
    [SerializeField] private float intervalo = 0.2f;

    [Tooltip("Excluye objetos que tienen Canvas (como UI de unidades).")]
    [SerializeField] private bool ignorarCanvas = true;

    private float proximaActualizacion;

    // Hacerlo accesible para invocar orden manualmente
    public static GestorOrdenVisualCamara Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (Time.time >= proximaActualizacion)
        {
            ActualizarOrdenes();
            proximaActualizacion = Time.time + intervalo;
        }
    }

    /// <summary>
    /// Reordena todos los objetos con el tag filtrado por posición Y.
    /// </summary>
    public void ActualizarOrdenes()
    {
        var objetos = GameObject.FindGameObjectsWithTag(tagFiltrado);

        List<(SpriteRenderer sr, float y)> lista = new();

        foreach (var obj in objetos)
        {
            if (ignorarCanvas && obj.GetComponentInChildren<Canvas>() != null)
                continue;

            foreach (var sr in obj.GetComponentsInChildren<SpriteRenderer>())
            {
                if (sr.enabled && sr.gameObject.activeInHierarchy)
                {
                    if (ignorarCanvas && sr.GetComponentInParent<Canvas>() != null)
                        continue;

                    lista.Add((sr, sr.transform.position.y));
                }
            }

        }

        var ordenados = lista.OrderBy(t => t.y).ToList();

        for (int i = 0; i < ordenados.Count; i++)
        {
            int orden = ordenados.Count - i; // invertimos
            ordenados[i].sr.sortingOrder = orden;
        }
    }
}
