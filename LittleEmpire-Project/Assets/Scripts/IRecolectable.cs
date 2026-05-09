using UnityEngine;
using System.Collections;

public interface IRecolectable
{
    TipoRecurso Tipo { get; }
    Transform PuntoDeRecoleccion { get; }
    IEnumerator EjecutarRecoleccion(Aldeano recolector);
}
