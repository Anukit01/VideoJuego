using UnityEngine;
using UnityEngine.AI;

public class Movimiento : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    public string zonaActual { get; private set; }
    public string ObtenerZonaActual() => zonaActual;



    [SerializeField] private GameObject indicadorSeleccion;
    [SerializeField] private GameObject indicadorDestino;
    [SerializeField] private string nombreAreaNavMesh = "Default"; // editable en inspector

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = 2f;
        agent.autoBraking = true;

        if (indicadorSeleccion != null)
            indicadorSeleccion.SetActive(false);
    }

    private void Update()
    {
        bool estaCerca = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
        animator.SetBool("isMoving", !estaCerca);

        if (indicadorDestino != null)
            indicadorDestino.SetActive(!estaCerca);

        OrientadorVisual orientador = GetComponent<OrientadorVisual>();
        if (orientador != null)
            orientador.GirarPorDireccion(agent.velocity);
    }

    public void MoverA(Vector3 destino)
    {
        int areaID = NavMesh.GetAreaFromName(nombreAreaNavMesh);
        int areaMask = 1 << areaID;

        if (NavMesh.SamplePosition(destino, out NavMeshHit hitNav, 0.3f, areaMask))
            agent.SetDestination(hitNav.position);
        else
            Debug.LogWarning("Destino fuera del NavMesh de la zona actual");

    }

    public void MostrarIndicadorSeleccion(bool activo)
    {
        if (indicadorSeleccion != null)
            indicadorSeleccion.SetActive(activo);
    }

    public void Detener()
    {
        agent.ResetPath();
        if (indicadorDestino != null)
            indicadorDestino.SetActive(false);
    }

 
  
    

 

}