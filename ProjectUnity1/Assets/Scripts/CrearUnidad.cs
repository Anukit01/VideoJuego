using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrearUnidad : MonoBehaviour
{
    public GameObject unidadArquero;
    public GameObject unidadAldeano;
    public GameObject unidadCaballero;
    public Transform puntoSpawn; // Punto donde aparece la unidad

    public void CrearAldeano1()
    {
        if (SeleccionadorDeUnidad.Instance.todasLasUnidades.Count >= GestionRecrsos.Instance.poblacion)
        {
            Debug.Log("No se puede crear más unidades. Límite de población alcanzado.");
            return;
        }
        else
        {

            StartCoroutine(CrearAldeano());
        }

    }
    public void CrearArquero1()
    {
        if (SeleccionadorDeUnidad.Instance.todasLasUnidades.Count >= GestionRecrsos.Instance.poblacion)
        {
            Debug.Log("No se puede crear más unidades. Límite de población alcanzado.");
            return;
        }
        else
        {
            StartCoroutine(CrearArquero());
        }
    }
    public void CrearCaballero1()
    {
        if (SeleccionadorDeUnidad.Instance.todasLasUnidades.Count >= GestionRecrsos.Instance.poblacion)
        {
            Debug.Log("No se puede crear más unidades. Límite de población alcanzado.");
            return;
        }
        else
        {
            StartCoroutine(CrearCaballero());
        }
    }
    public IEnumerator CrearArquero()
    {
      
        if (unidadArquero == null)
        {
            Debug.LogError("Error: unidadPrefab no está asignado en el Inspector.");
            yield break;
        }

        if (!unidadArquero.TryGetComponent<Arquero>(out var construible))
        {
            Debug.LogError("Error: el prefab no tiene componente.");
            yield break;
        }


        foreach (var costo in construible.Costos)
        {


            if (!GestionRecrsos.Instance.TieneRecurso(costo.nombreRecurso, costo.cantidad))
            {
                Debug.LogWarning("Faltan recursos al momento de crear");
                yield break;
            }
        }

        foreach (var costo in construible.Costos)
        {

            GestionRecrsos.Instance.GastarRecurso(costo.nombreRecurso, costo.cantidad);
        }


        Instantiate(unidadArquero, puntoSpawn.position, Quaternion.identity);
    }
    public IEnumerator CrearAldeano()
    {
        if (unidadAldeano == null)
        {
            Debug.LogError("Error: unidadPrefab no está asignado en el Inspector.");
            yield break;
        }

        if (!unidadAldeano.TryGetComponent<Aldeano>(out var construible))
        {
            Debug.LogError("Error: el prefab no tiene componente CharactBase.");
            yield break;
        }
        

        foreach (var costo in construible.Costos)
        {
            

            if (!GestionRecrsos.Instance.TieneRecurso(costo.nombreRecurso, costo.cantidad))
            {
                Debug.LogWarning("Faltan recursos al momento de crear");
                yield break;
            }
        }
       
        foreach (var costo in construible.Costos)
        {
            
            GestionRecrsos.Instance.GastarRecurso(costo.nombreRecurso, costo.cantidad);
        }
       

        Instantiate(unidadAldeano, puntoSpawn.position, Quaternion.identity);
    }

    public IEnumerator CrearCaballero()
    {
        if (unidadCaballero == null)
        {
            Debug.LogError("Error: unidadPrefab no está asignado en el Inspector.");
            yield break;
        }

        if (!unidadCaballero.TryGetComponent<Caballero>(out var construible))
        {
            Debug.LogError("Error: el prefab no tiene componente CharactBase.");
            yield break;
        }


        foreach (var costo in construible.Costos)
        {


            if (!GestionRecrsos.Instance.TieneRecurso(costo.nombreRecurso, costo.cantidad))
            {
                Debug.LogWarning("Faltan recursos al momento de crear");
                yield break;
            }
        }

        foreach (var costo in construible.Costos)
        {

            GestionRecrsos.Instance.GastarRecurso(costo.nombreRecurso, costo.cantidad);
        }


        Instantiate(unidadCaballero, puntoSpawn.position, Quaternion.identity);
    }

}
