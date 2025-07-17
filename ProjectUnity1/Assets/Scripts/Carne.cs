using UnityEngine;
using System.Collections;
using System.Linq;

public class Carne : MonoBehaviour, IRecolectable
{

    [SerializeField] public float radioRecoleccion = 1.5f;

    public int cantidad = 60;
    public TipoRecurso Tipo => TipoRecurso.Alimento;
    public Transform PuntoDeRecoleccion => transform;

    void Start()
    {
        StartCoroutine(EsperarYAtraerAldeano());
    }
    private IEnumerator EsperarYAtraerAldeano()
    {
        yield return new WaitForSeconds(0.1f); // esperamos que se instancie bien

        var candidato = GameObject.FindObjectsOfType<Aldeano>()
            .FirstOrDefault(a => a.estadoActual == Aldeano.EstadoAldeano.Idle);

        if (candidato != null)
        {
            candidato.EjecutarAccion(gameObject, transform.position);
        }
    }
    public IEnumerator EjecutarRecoleccion(Aldeano aldeano)
    {
        Debug.Log("Recolectando carne, queda: " + cantidad);
       
        yield return new WaitForSeconds(0.5f);
        Recolectar(aldeano);
        aldeano.TerminarRecoleccion();
    }
    public bool EstaEnRango(Vector3 posicion)
    {
        return Vector3.Distance(transform.position, posicion) <= radioRecoleccion;
    }

    public void Recolectar(Aldeano aldeano)
    {
        int cantidadExtraida = 10;

        if (cantidad <= 0) return;

        cantidad -= cantidadExtraida;

        aldeano.CargaRecoleccion.visual = aldeano.VisualCarne;
        aldeano.CargaRecoleccion.tipo = TipoRecurso.Alimento;
        aldeano.CargaRecoleccion.cantidad = cantidadExtraida;
        //ultimoRecurso = recurso;


        if (cantidad <= 0)
            Destroy(gameObject);
    }
}