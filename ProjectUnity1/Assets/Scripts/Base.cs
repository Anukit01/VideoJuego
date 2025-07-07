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

    public override void InicializarVida(int cantidad)
    {
        if (vidaMaxima <= 0)
        {
            Debug.LogWarning($" vidaMaxima no inicializada en '{name}'");
            vidaMaxima = 1; // fallback seguro
        }

        vida = Mathf.Clamp(cantidad, 0, vidaMaxima);
        ActualizarVidaVisual();
    }

    public override void CompleteConstruction()
    {
        base.CompleteConstruction();
        if (vidaVisual != null)
            vidaVisual.SetActive(false);
        
        GestionRecrsos.Instance.SumarPoblación(3);
    }

      private void OnMouseDown()
    {
        if (vidaVisual != null)
            vidaVisual.SetActive(!vidaVisual.activeSelf);

        if (!EstáConstruido)
            return;

        if (menuEdificio != null)
            menuEdificio.SetActive(!menuEdificio.activeSelf);
    }

}

