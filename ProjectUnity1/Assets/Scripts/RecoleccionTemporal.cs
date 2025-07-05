using UnityEngine;

public class RecoleccionTemporal
{
    public TipoRecurso tipo;
    public int cantidad = 0;
    public int capacidad = 10;
    public GameObject visual;
    public bool TieneCarga => cantidad > 0;


    public void Vaciar() => cantidad = 0;
    public bool EstaLleno => cantidad >= capacidad;
}