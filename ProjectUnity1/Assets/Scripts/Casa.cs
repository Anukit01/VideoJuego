using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Casa : EdificioBase
{

    public GameObject vidaVisual;

    protected override void Start()
    {
        InicializarVida(0);
        vidaMaxima = 80;
        defensa = 5;
        construido = false;
        BeginConstruction();
        if (vidaVisual != null)
            vidaVisual.SetActive(true);
        GestorEntidades.Instance.RegistrarCasa();
    }
 
    public override void CompleteConstruction()
    {
        base.CompleteConstruction();
        if (vidaVisual != null)
            vidaVisual.SetActive(false);
        GestionRecrsos.Instance.SumarPoblación(2);
    }
    public void ActivarVisuales()
    {
        if (vidaVisual != null)
            vidaVisual.SetActive(!vidaVisual.activeSelf);

    }
    public override void Derribar()
    {
        GestorEntidades.Instance.CasaDestruida();
        GestionRecrsos.Instance.SumarPoblación(-2);
        base.Derribar();
    }
}
