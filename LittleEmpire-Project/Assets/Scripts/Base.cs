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
        if (menuEdificio == null)
        {
            // Busca el hijo llamado "MenuEdificio" dentro del prefab
            menuEdificio = transform.Find("MenuEdificio")?.gameObject;
        }
        GestorEntidades.Instance.RegistrarBase();
    }
   void Update()
    {
        if (menuEdificio != null && menuEdificio.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                // Si el click no fue sobre la base ni sobre ningún hijo de la base (incluido el menú), cerramos
                if (hit.collider == null || !hit.collider.transform.IsChildOf(transform))
                {
                    ActivarMenuVida();
                }
            }
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
        GestorEntidades.Instance.BaseDestruida();
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
    public void ActivarMenuVida()
    {
        if (!EstáConstruido)
            return;
        
        if (vidaVisual != null)
            vidaVisual.SetActive(!vidaVisual.activeSelf);       

        if (menuEdificio != null)
            menuEdificio.SetActive(!menuEdificio.activeSelf);
    }

}

