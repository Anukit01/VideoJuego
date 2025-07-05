using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class UnidadJugador : UnidadBase, IAccionContextual
{
    protected NavMeshAgent agent;
    protected Animator animator;
    protected List<CostoEdificio> costos;
    public virtual List<CostoEdificio> Costos => costos;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = 0.05f;
        agent.autoBraking = true;

        SeleccionadorDeUnidad.Instance?.todasLasUnidades.Add(gameObject);
    }

    public abstract void EjecutarAccion(GameObject objetivo, Vector3 destino);
}