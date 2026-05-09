using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class UnidadJugador : UnidadBase, IAccionContextual
{
    protected NavMeshAgent agent;
    protected Animator animator;
    [SerializeField] protected List<CostoEdificio> costos;
    public virtual List<CostoEdificio> Costos => costos;

    protected override void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = 0.7f;
        agent.autoBraking = true;
        base.Start();
        
    }

    protected override void Morir()
    {
        StopAllCoroutines();
        SeleccionadorDeUnidad.Instance?.Deseleccionar(gameObject); //  removemos de selección
        SeleccionadorDeUnidad.Instance?.todasLasUnidades.Remove(gameObject); // removemos de la lista de todas las unidades
        animator.SetTrigger("Morir"); // la animación debe estar como trigger
        Destroy(gameObject, 1.7f); // da tiempo a la animación antes de desaparecer
        GestionRecrsos.Instance.ActualizarUI();
    }

}