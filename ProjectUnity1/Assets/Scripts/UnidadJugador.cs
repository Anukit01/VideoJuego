using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class UnidadJugador : UnidadBase, IAccionContextual
{
    protected NavMeshAgent agent;
    protected Animator animator;
    protected List<CostoEdificio> costos;
    public virtual List<CostoEdificio> Costos => costos;

    protected override void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = 0.05f;
        agent.autoBraking = true;
        base.Start();
        
    }

    protected override void Morir()
    {
        animator.SetTrigger("Morir"); // la animación debe estar como trigger
        SeleccionadorDeUnidad.Instance?.Deseleccionar(gameObject); //  removemos de selección

        Destroy(gameObject, 1.7f); // da tiempo a la animación antes de desaparecer
    }

}