using UnityEngine;

public abstract class EntidadBase : MonoBehaviour, IAtacable
{
    public Faccion faccion;
    [SerializeField] protected int vidaMaxima;
    protected int vida;

    public int VidaMaxima => vidaMaxima;
    public int VidaActual => vida;

    public void InicializarVida(int cantidad)
    {
        vidaMaxima = cantidad;
        vida = cantidad;
        ActualizarVidaVisual();
    }

    protected virtual void Start()
    {
        InicializarVida(vidaMaxima);
        // Solo las unidades del jugador deben registrarse como parte del sistema de selección múltiple
        if (this is UnidadJugador)
        {
            SeleccionadorDeUnidad.Instance?.todasLasUnidades.Add(gameObject);
        }


    }



    public virtual bool EstaVivo() => vida > 0;

    public virtual void RecibirDanio(int cantidad)
    {
        vida -= cantidad;
        ActualizarVidaVisual();
        if (vida <= 0)
            Morir();
    }

    protected virtual void Morir()
    {
        Destroy(gameObject);
    }
    protected void ActualizarVidaVisual()
    {
        var barra = GetComponentInChildren<VidaVisual>();
        if (barra != null)
            barra.ActualizarVidaVisual();
    }
}