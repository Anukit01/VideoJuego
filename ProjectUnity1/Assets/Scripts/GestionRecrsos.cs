using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestionRecrsos : MonoBehaviour
{
    public static GestionRecrsos Instance; 
    private Dictionary<string, int> recursos = new Dictionary<string, int>();

    public int alimento = 50;
    public int oro = 25;
    public int madera = 100;
    public int poblacion = 0;
 

    public Text TextAlimento;
    public Text TextOro;
    public Text TextMadera;
    public Text TextPoblacion;
  

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Asigna la instancia única
        }
        else
        {
            Destroy(gameObject); // Si ya existe otra instancia, elimina la nueva
        }
    }
     private void Start()
    {
        recursos["alimento"] = alimento;
        recursos["oro"] = oro;
        recursos["madera"] = madera;

        ActualizarUI();
    }

    public void ActualizarUI()
    {
        TextAlimento.text = "Alimento: " + alimento;
        TextOro.text = "Oro: " + oro;
        TextMadera.text = "Madera: " + madera;
        TextPoblacion.text = $"Población: {SeleccionadorDeUnidad.Instance.todasLasUnidades.Count} / {GestionRecrsos.Instance.poblacion}";
    }
    public bool TieneRecurso(string nombre, int cantidad)
    {
        return recursos.ContainsKey(nombre.ToLower()) && recursos[nombre.ToLower()] >= cantidad;
    }
    public void GastarRecurso(string nombre, int cantidad)
    {
        if (recursos.ContainsKey(nombre.ToLower()))
        {
            recursos[nombre.ToLower()] -= cantidad;


            switch (nombre.ToLower())
            {
                case "madera": madera = recursos["madera"]; break;
                case "oro": oro = recursos["oro"]; break;
                case "alimento": alimento = recursos["alimento"]; break;
            }
            ActualizarUI();
        }
    }

    public void RestarRecursos(int costoAlimento, int costoOro, int costoMadera)
    {
        alimento -= costoAlimento;
        oro -= costoOro;
        madera -= costoMadera;
        ActualizarUI();
    }

    public void SumarMadera(int cantidad)
    {
        madera += cantidad;
        recursos["madera"] = madera;
        ActualizarUI();
    }

    public void SumarAlimento(int cantidad)
    {
        alimento += cantidad;
        recursos["alimento"] = madera;
        ActualizarUI(); // Refresca el menú de recursos
    }

    internal void SumarOro(int cantidad)
    {
       oro += cantidad;
        recursos["oro"] = madera;
        ActualizarUI();
    }
    internal void SumarPoblación(int cantidad)
    {
        poblacion += cantidad;
        recursos["población"] = madera;
        ActualizarUI();
    }
}


