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
        
    }

    private void OnMouseDown()
    {
        vidaVisual.SetActive(!vidaVisual.activeSelf);

    }
    public override void CompleteConstruction()
    {
        base.CompleteConstruction();
        if (vidaVisual != null)
            vidaVisual.SetActive(false);
        GestionRecrsos.Instance.SumarPoblación(2);
    }
}
