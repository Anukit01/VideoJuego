using UnityEngine;
using UnityEngine.AI;

public class Movimiento : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [SerializeField] private GameObject indicadorSeleccion;
    [SerializeField] private GameObject indicadorDestino;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = 0.05f;
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
        if (NavMesh.SamplePosition(destino, out NavMeshHit hitNav, 0.3f, NavMesh.AllAreas))
            agent.SetDestination(hitNav.position);
        else
            Debug.LogWarning("Destino fuera del NavMesh");
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