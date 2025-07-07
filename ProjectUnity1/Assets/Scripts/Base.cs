using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Base : EdificioBase
{
    public GameObject menuEdificio;
    public GameObject vidaVisual;

    protected override void Start()
    {
        InicializarVida(170); 
        BeginConstruction();
        base.Start();
        
    }
    
    public override void CompleteConstruction()
    {
        base.CompleteConstruction();
        GestionRecrsos.Instance.SumarPoblación(3);
    }

    private void OnMouseDown()
    {
        
        
        vidaVisual.SetActive(!vidaVisual.activeSelf);

        if (!construido)
            return;

        menuEdificio.SetActive(!menuEdificio.activeSelf);



    }
}
