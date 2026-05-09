using System.Collections.Generic;
using UnityEngine;

public class CharactBase : MonoBehaviour, IAtacable
{
    public bool EstaVivo() => vida > 0;
    public Faccion faccion;

    public int vida;
    public int ataque;
    public int defensa;
    public float velocidad;
    public List<CostoEdificio> Costos { get; }
    protected virtual void Start()
    {
        if (SeleccionadorDeUnidad.Instance != null)
            SeleccionadorDeUnidad.Instance.todasLasUnidades.Add(gameObject);
    }
    public enum DireccionAtaque
    {
        Izquierda,
        Derecha,
        Arriba,
        Abajo
    }

    public DireccionAtaque CalcularDireccion(Vector2 origen, Vector2 destino)
    {
        Vector2 direccion = destino - origen;
        float absX = Mathf.Abs(direccion.x);
        float absY = Mathf.Abs(direccion.y);

        if (absX > absY)
            return direccion.x >= 0 ? DireccionAtaque.Derecha : DireccionAtaque.Izquierda;
        else
            return direccion.y >= 0 ? DireccionAtaque.Arriba : DireccionAtaque.Abajo;
    }


    public void RecibirDanio(int cantidad)
    {
        vida -= cantidad;
        if (vida <= 0) Morir();
    }
    private void Morir()
    {
        Destroy(gameObject);
    }

    public void Atacar(GameObject objetivo)
    {
        if (objetivo.TryGetComponent<IAtacable>(out IAtacable atacable))
        {
            atacable.RecibirDanio(ataque, gameObject);
            
        }
    }

    public void RecibirDanio(int cantidad, GameObject atacante)
    {
        throw new System.NotImplementedException();
    }
}