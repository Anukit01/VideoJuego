using UnityEngine;

public abstract class UnidadBase : EntidadBase
{
    public int ataque = 10;
    public int defensa = 5;
    public float velocidad = 5f;
    public bool respondeAlAtaque = true;


    public abstract void EjecutarAccion(GameObject objetivo, Vector3 destino);
}