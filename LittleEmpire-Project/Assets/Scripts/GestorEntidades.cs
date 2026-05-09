using System.Collections.Generic;
using UnityEngine;

public class GestorEntidades : MonoBehaviour
{
    public static GestorEntidades Instance { get; private set; }

    public int aldeanosCreados = 0;
    public int aldeanosMuertos = 0;
    public int arquerosCreados = 0;
    public int arquerosMuertos = 0;
    public int caballerosCreados = 0;
    public int caballerosMuertos = 0;

    public int basesCreadas = 0;
    public int basesDestruidas = 0;
    public int casasCreadas = 0;
    public int casasDestruidas = 0;
    public int torresCreadas = 0;
    public int torresDestruidas = 0;

    void Awake()
    {
        Instance = this;
    }

    public void RegistrarAldeano()
    {
        aldeanosCreados++;
    }
    public void AldeanoMuerto()
    {
        aldeanosMuertos++;
        GestorVictoria.Instance.VerificarDerrota();
    }
    public void RegistrarArquero()
    {
        arquerosCreados++;
    }
    public void ArqueroMuerto()
    {
        arquerosMuertos++;
    }
    public void RegistrarCaballero()
    {
        caballerosCreados++;
    }
    public void CaballeroMuerto()
    {
        caballerosMuertos++ ;
    }

    public void RegistrarBase()
    {
        basesCreadas++;
    }
    public void BaseDestruida()
    {
        basesDestruidas++;
        GestorVictoria.Instance.VerificarDerrota();
    }
    public void RegistrarCasa()
    {
        casasCreadas++;
    }
    public void CasaDestruida()
    {
        casasDestruidas++;
    }
    public void RegistrarTorre()
    {
        torresCreadas++;
    }
    public void TorreDestruida()
    {
        torresDestruidas++;
    }

}
