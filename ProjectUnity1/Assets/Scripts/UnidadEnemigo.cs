using UnityEngine;
using UnityEngine.AI;

public abstract class UnidadEnemigo : UnidadBase
{
    protected NavMeshAgent agent;
    protected Transform objetivoActual;
    protected float tiempoEntreAcciones = 1f;
    protected float proximoMovimiento = 0f;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = 0.5f;
    }

    protected virtual void Update()
    {
        if (!EstaVivo()) return;

        if (Time.time >= proximoMovimiento)
        {
            EjecutarIA();
            proximoMovimiento = Time.time + tiempoEntreAcciones;
        }
    }

    // Este método lo sobrescribís en cada enemigo para darle su lógica
    protected abstract void EjecutarIA();

    // Para moverlo con NavMesh
    protected void MoverHacia(Vector3 destino)
    {
        if (agent != null)
            agent.SetDestination(destino);
    }

    // O directamente con Transform si no usás NavMesh
    protected void MoverSimple(Vector3 destino, float velocidadMovimiento)
    {
        transform.position = Vector2.MoveTowards(transform.position, destino, velocidadMovimiento * Time.deltaTime);
    }
}
