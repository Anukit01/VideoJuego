using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Base : EdificioBase
{
    public GameObject menuEdificio;
    public GameObject vidaVisual;
    [SerializeField] private bool iniciarConstruido = false;


   
    protected override void Start()
    {
            
        vidaMaxima = 250;
        defensa = 9;
        if (!iniciarConstruido)
        {
            InicializarVida(0);
            construido = false;
            BeginConstruction();
        }
        else
        { 
            InicializarVida(vidaMaxima);
            construido = true;
            CompleteConstruction(); 
        }     
    }
   

    public override void BeginConstruction()
    {
        base.BeginConstruction();
        var puntosEntrega = gameObject.GetComponents<PuntoDeEntrega>();
        foreach (var punto in puntosEntrega)
        {
            punto.enabled = false;
        }

    }
    public override void InicializarVida(int cantidad)
    {
        if (vidaMaxima <= 0)
        {
            Debug.LogWarning($" vidaMaxima no inicializada en '{name}'");
            vidaMaxima = 1;
        }

        vida = Mathf.Clamp(cantidad, 0, vidaMaxima);
        ActualizarVidaVisual();

       
    }

    public override void CompleteConstruction()
    {
        
        base.CompleteConstruction();

        if (vidaVisual != null)
            vidaVisual.SetActive(false);

        var puntosEntrega = gameObject.GetComponents<PuntoDeEntrega>();
        foreach (var punto in puntosEntrega)
        {
            punto.enabled = true;
        }
        GestionRecrsos.Instance.SumarPoblación(3);
    }
    public override void Derribar()
    {
       
        if (!EstáConstruido && gameObject.TryGetComponent<PuntoDeEntrega>(out var puntoEntrega))
        {
            puntoEntrega.enabled = false;
        }
        GestionRecrsos.Instance.SumarPoblación(-3);
        ActualizarVidaVisual();
        GestionRecrsos.Instance.ActualizarUI();
        
        base.Derribar();


    }

    public void ActivarVisuales()
    {
        if (vidaVisual != null)
            vidaVisual.SetActive(!vidaVisual.activeSelf);

        if (!EstáConstruido)
            return;

        if (menuEdificio != null)
            menuEdificio.SetActive(!menuEdificio.activeSelf);
    }



}

