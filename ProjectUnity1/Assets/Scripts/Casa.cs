using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casa : EdificioBase
{
    protected override void Start()
    {
        InicializarVida(50); // o la vida que quieras
        BeginConstruction();
        base.Start();
    }


    public override void CompleteConstruction()
    {
        base.CompleteConstruction();
        GestionRecrsos.Instance.SumarPoblación(2);
    }
}
