using UnityEngine;
using System.Collections;

public class Carne : MonoBehaviour, IRecolectable
{
    public int cantidadCarne = 10;
    public TipoRecurso Tipo => TipoRecurso.Alimento;
    public Transform PuntoDeRecoleccion => transform;

    public IEnumerator EjecutarRecoleccion(Aldeano aldeano)
    {
        // Recolección instantánea
        aldeano.CargaRecoleccion.cantidad = cantidadCarne;

        yield return null;
        aldeano.TerminarRecoleccion();

        Destroy(gameObject); // carne desaparece al recogerla
    }
}