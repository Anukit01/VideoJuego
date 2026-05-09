using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IBuilding
{
    void BeginConstruction();
    void CompleteConstruction();
    bool Est·Construido { get; }
    Transform GetPuntoConstruccion();
    IEnumerator ProcesoConstruccion(System.Action onConstruccionTerminada);
    float TiempoConstruccion { get; }
    List<CostoEdificio> Costos { get; }
}
