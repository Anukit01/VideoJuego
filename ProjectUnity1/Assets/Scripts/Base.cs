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
            
        vidaMaxima = 150;
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
        if (gameObject.TryGetComponent<PuntoDeEntrega>(out var puntoEntrega))
        {
            puntoEntrega.enabled = false;
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

        if (Est�Construido && gameObject.TryGetComponent<PuntoDeEntrega>(out var puntoEntrega))
        {
            puntoEntrega.enabled = true;
        }
    }

    public override void CompleteConstruction()
    {
        
        base.CompleteConstruction();

        if (vidaVisual != null)
            vidaVisual.SetActive(false);

        if (gameObject.TryGetComponent<PuntoDeEntrega>(out var puntoEntrega))
            puntoEntrega.enabled = true; // ahora s� est� construido 

        GestionRecrsos.Instance.SumarPoblaci�n(3);
    }
    public override void Derribar()
    {
       
        if (!Est�Construido && gameObject.TryGetComponent<PuntoDeEntrega>(out var puntoEntrega))
        {
            puntoEntrega.enabled = false;
        }
        GestionRecrsos.Instance.SumarPoblaci�n(-3);
        ActualizarVidaVisual();
        GestionRecrsos.Instance.ActualizarUI();
        
        base.Derribar();


    }

    public void ActivarVisuales()
    {
        if (vidaVisual != null)
            vidaVisual.SetActive(!vidaVisual.activeSelf);

        if (!Est�Construido)
            return;

        if (menuEdificio != null)
            menuEdificio.SetActive(!menuEdificio.activeSelf);
    }



}

