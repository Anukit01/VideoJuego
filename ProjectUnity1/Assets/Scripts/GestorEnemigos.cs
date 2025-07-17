using UnityEngine;

public class GestorEnemigos : MonoBehaviour
{
    public static GestorEnemigos Instance { get; private set; }

    private int enemigosTotales = 0;
    private int enemigosDerrotados = 0;
    private bool condicionActivada = false;

    void Awake()
    {
        Instance = this;
    }

    public void RegistrarEnemigo()
    {
        enemigosTotales++;
    }

    public void NotificarMuerte()
    {
        enemigosDerrotados++;

        float ratio = (float)enemigosDerrotados / enemigosTotales;

        if (ratio >= 0.75f && !condicionActivada)
        {
            condicionActivada = true;
            GestorVictoria.Instance.ActivarVictoriaParcial();
        }

        if (enemigosDerrotados >= enemigosTotales)
        {
            GestorVictoria.Instance.VictoriaFinal();
        }
    }
}