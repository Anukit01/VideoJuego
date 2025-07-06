using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Base : EdificioBase
{
    public GameObject menuEdificio;

    private void Start()
    {
        BeginConstruction();
    }

    public override void CompleteConstruction()
    {
        base.CompleteConstruction();
        GestionRecrsos.Instance.SumarPoblación(3);
    }

    private void OnMouseDown()
    {
        menuEdificio.SetActive(!menuEdificio.activeSelf);
    }
}
