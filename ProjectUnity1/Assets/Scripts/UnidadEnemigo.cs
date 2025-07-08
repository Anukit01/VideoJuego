using UnityEngine;
using UnityEngine.AI;

public abstract class UnidadEnemigo : UnidadBase
{
    protected NavMeshAgent agent;
    protected Transform objetivoActual;
    protected float tiempoEntreAcciones = 1f;
    protected float proximoMovimiento = 0f;
    protected Animator animator;

    protected override void Start()

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
        {
            animator.SetBool("IsMoving", true);
            agent.SetDestination(destino);

            Vector2 direccion = destino - transform.position;
            GetComponent<OrientadorVisual>()?.GirarPorDireccion(direccion);
        }

    }
    protected override void Morir()
    {
        animator.SetTrigger("Morir"); // la animación debe estar como trigger

        Destroy(gameObject, 1f); // da tiempo a la animación antes de desaparecer
    }


}
