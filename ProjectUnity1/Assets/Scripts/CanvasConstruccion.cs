using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CanvasConstruccion : MonoBehaviour
{
    [SerializeField] private GameObject prefabCastillo;
    [SerializeField] private GameObject prefabCastilloFantasma;

    [SerializeField] private GameObject prefabTorre;
    [SerializeField] private GameObject prefabTorreFantasma;

    [SerializeField] private GameObject prefabCasa;
    [SerializeField] private GameObject prefabCasaFantasma;


    private bool TieneRecursosSuficientes(IBuilding edificio)
    {
        foreach (var costo in edificio.Costos)
        {
            if (!GestionRecrsos.Instance.TieneRecurso(costo.nombreRecurso, costo.cantidad))
                return false;
        }
        return true;
    }

    public void ConstruirCastillo()
    {
        var castillo = prefabCastillo.GetComponent<IBuilding>();

        if (castillo != null && TieneRecursosSuficientes(castillo))
        {
            BuildingPlacementManager.Instance.BeginPlacement(prefabCastillo, prefabCastilloFantasma);
        }
        else
        {
            Debug.Log("No hay suficientes recursos para el Castillo.");
        }
    }

    public void ConstruirTorre()
    {
        var torre = prefabTorre.GetComponent<IBuilding>();

        if (torre != null && TieneRecursosSuficientes(torre))
        {
            BuildingPlacementManager.Instance.BeginPlacement(prefabTorre, prefabTorreFantasma);
        }
        else
        {
            Debug.Log("No hay suficientes recursos para el Castillo.");
        }      
    }

    public void ConstruirCasa()
    {
        var casa = prefabCasa.GetComponent<IBuilding>();

        if (casa != null && TieneRecursosSuficientes(casa))
        {
            BuildingPlacementManager.Instance.BeginPlacement(prefabCasa, prefabCasaFantasma);
        }
        else
        {
            Debug.Log("No hay suficientes recursos para el Castillo.");
        }
    }
}

