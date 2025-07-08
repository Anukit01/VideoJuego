using UnityEngine;

public abstract class EntidadBase : MonoBehaviour, IAtacable
{
    public Faccion faccion;
    [SerializeField] protected int vidaMaxima;
    [SerializeField] protected int vida;

    public int VidaMaxima => vidaMaxima;
    public int VidaActual => vida;

    public virtual void InicializarVida(int cantidad)
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

    public virtual void RecibirDanio(int cantidad, GameObject atacante)
    {
        int defensaLocal = 0;
        if (this is UnidadBase unidadDefensora)
            defensaLocal = unidadDefensora.defensa;

        vida -= Mathf.Max(cantidad - defensaLocal, 0);
        ActualizarVidaVisual();

        if (vida <= 0)
            Morir();

        if (this is UnidadBase unidad && unidad.respondeAlAtaque && unidad.EstaVivo())
        {
            float distancia = Vector2.Distance(transform.position, atacante.transform.position);
            if (distancia < 6f)
                unidad.EjecutarAccion(atacante, atacante.transform.position);
        }


    }


    protected virtual void Morir()
    {
       
    }
    protected void ActualizarVidaVisual()
    {
        var barra = GetComponentInChildren<VidaVisual>();
        if (barra != null)
            barra.ActualizarVidaVisual();
    }
}