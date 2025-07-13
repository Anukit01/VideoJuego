using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casa : EdificioBase
{
    public GameObject vidaVisual;
    protected override void Start()
    {
        InicializarVida(0);
        vidaMaxima = 50;
        construido = false;
        BeginConstruction();
        if (vidaVisual != null)
            vidaVisual.SetActive(true);
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
}
