using System.Collections;
using UnityEngine;

public class Oro : MonoBehaviour, IRecolectable
{
    public int cantidad = 30;
    public GameObject oroRecolectadoPrefab;
    [SerializeField] private GameObject visualInactiva;
    [SerializeField] private GameObject visualActiva;
    [SerializeField] private GameObject visualDestruida;
    [SerializeField] private Transform puntoDeRecoleccion;
    public Transform PuntoDeRecoleccion => puntoDeRecoleccion;

   
    public TipoRecurso Tipo => TipoRecurso.Oro;

    public IEnumerator EjecutarRecoleccion(Aldeano aldeano)
    {
        yield return new WaitForSeconds(0.5f); // Animación opcional
        Recolectar(aldeano);
        aldeano.TerminarRecoleccion();
    }


    public void Recolectar(Aldeano aldeano)
    {
        int cantidadExtraida = 5;

        if (cantidad <= 0) return;

        cantidad -= cantidadExtraida;
        MostrarVisualActiva();

        aldeano.CargaRecoleccion.visual = aldeano.VisualBolsaOro;
        aldeano.CargaRecoleccion.tipo = TipoRecurso.Oro;
        aldeano.CargaRecoleccion.cantidad = cantidadExtraida;


        if (cantidad <= 0)
            MostrarVisualDestruida();
    }

    public void RevertirVisualActiva()
    {
        visualActiva.SetActive(false);
        visualInactiva.SetActive(true);
    }

    public void MostrarVisualActiva()
    {
        visualInactiva.SetActive(false);
        visualActiva.SetActive(true);
    }

   public void MostrarVisualDestruida()
    {
        visualActiva.SetActive(false);
        visualDestruida.SetActive(true);
        // Podés hacer que el objeto ya no sea seleccionable
    }
}
