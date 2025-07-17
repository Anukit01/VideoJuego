using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrearUnidad : MonoBehaviour
{
    public GameObject unidadArquero;
    public GameObject unidadAldeano;
    public GameObject unidadCaballero;
    public Transform puntoSpawn; // Punto donde aparece la unidad

    
    public string zonaActual { get; private set; }




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


        GameObject nuevoArquero = Instantiate(unidadArquero, puntoSpawn.position, Quaternion.identity);
        GestorEntidades.Instance.Registrar("Arquero", nuevoArquero);

        GestionRecrsos.Instance.ActualizarUI();
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


        GameObject nuevoAldeano = Instantiate(unidadAldeano, puntoSpawn.position, Quaternion.identity);
        GestorEntidades.Instance.Registrar("Aldeano", nuevoAldeano);
        GestionRecrsos.Instance.ActualizarUI();


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


        GameObject nuevoCaballero = Instantiate(unidadCaballero, puntoSpawn.position, Quaternion.identity);
        GestorEntidades.Instance.Registrar("Caballero", nuevoCaballero);

        GestionRecrsos.Instance.ActualizarUI();


    }

}
