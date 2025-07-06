using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casa : EdificioBase
{
    private void Start()
    {
        BeginConstruction();
    }

    public override void CompleteConstruction()
    {
        base.CompleteConstruction();
        GestionRecrsos.Instance.SumarPoblación(2);
    }
}
