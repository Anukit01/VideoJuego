[System.Serializable]
public class CostoEdificio
{
    public string nombreRecurso;
    public int cantidad;

    public CostoEdificio(string recurso, int cantidad)
    {
        this.nombreRecurso = recurso;
        this.cantidad = cantidad;
    }
}
