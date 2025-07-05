using UnityEngine;

public abstract class EntidadBase : MonoBehaviour, IAtacable
{
    public Faccion faccion;
    public int vida = 100;

    public virtual bool EstaVivo() => vida > 0;

    public virtual void RecibirDanio(int cantidad)
    {
        vida -= cantidad;
        if (vida <= 0)
            Morir();
    }

    protected virtual void Morir()
    {
        Destroy(gameObject);
    }
}