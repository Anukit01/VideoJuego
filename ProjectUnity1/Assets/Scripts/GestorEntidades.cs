using System.Collections.Generic;
using UnityEngine;

public class GestorEntidades : MonoBehaviour
{
    public static GestorEntidades Instance { get; private set; }

    private Dictionary<string, List<GameObject>> entidades = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Registrar(string tipo, GameObject entidad)
    {
        if (!entidades.ContainsKey(tipo))
            entidades[tipo] = new List<GameObject>();

        entidades[tipo].Add(entidad);
    }

    public void Eliminar(string tipo, GameObject entidad)
    {
        if (entidades.ContainsKey(tipo))
            entidades[tipo].Remove(entidad);
    }

    public int Contar(string tipo)
    {
        if (entidades.ContainsKey(tipo))
            return entidades[tipo].Count;

        return 0;
    }
}
