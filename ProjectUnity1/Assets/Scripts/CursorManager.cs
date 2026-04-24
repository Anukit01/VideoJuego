using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorDefault;
    public Texture2D cursorHacha;
    public Texture2D cursorPico;
    public Texture2D cursorPastor;
    public Texture2D cursorEspada;

    private Vector2 hotspot = Vector2.zero;

    void Update()
    {
        
        if (SeleccionadorDeUnidad.Instance.unidadesSeleccionadas.Count == 0)
        {
            Cursor.SetCursor(cursorDefault, hotspot, CursorMode.Auto);
            return;
        }

        // Detectar qué hay bajo el mouse
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if (hit == null)
        {
            Cursor.SetCursor(cursorDefault, hotspot, CursorMode.Auto);
            return;
        }

        
        bool hayAldeano = SeleccionadorDeUnidad.Instance.unidadesSeleccionadas
            .Exists(u => u.GetComponent<Aldeano>() != null);

        if (hayAldeano)
        {
            if (hit.GetComponent<Tree>() != null)
                Cursor.SetCursor(cursorHacha, hotspot, CursorMode.Auto);
            else if (hit.GetComponent<Oro>() != null)
                Cursor.SetCursor(cursorPico, hotspot, CursorMode.Auto);
            else if (hit.GetComponent<Sheep>() != null)
                Cursor.SetCursor(cursorPastor, hotspot, CursorMode.Auto);
            else if (hit.GetComponent<IAtacable>() != null && EsEnemigo(hit.gameObject))
                Cursor.SetCursor(cursorEspada, hotspot, CursorMode.Auto);
            else
                Cursor.SetCursor(cursorDefault, hotspot, CursorMode.Auto);
        }
        else
        {
            
            if (hit.GetComponent<IAtacable>() != null && EsEnemigo(hit.gameObject))
                Cursor.SetCursor(cursorEspada, hotspot, CursorMode.Auto);
            else
                Cursor.SetCursor(cursorDefault, hotspot, CursorMode.Auto);
        }
    }
    private bool EsEnemigo(GameObject objetivo)
    {
        
        if (objetivo.TryGetComponent<EntidadBase>(out var entidad))
        {
            
            var faccionSeleccionada = SeleccionadorDeUnidad.Instance.unidadesSeleccionadas[0]
                .GetComponent<EntidadBase>().faccion;

            return FaccionUtils.SonEnemigos(faccionSeleccionada, entidad.faccion);
        }

        return false; 
    }
}
