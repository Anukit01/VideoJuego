using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorDefault;
    public Texture2D cursorHacha;
    public Texture2D cursorPico;
    public Texture2D cursorPastor;
    public Texture2D cursorEspada;

    private void Update()
    {
        if (SeleccionadorDeUnidad.Instance.unidadesSeleccionadas.Count == 0)
        {
            SetCursor(cursorDefault);
            return;
        }

        // Detectar qué hay bajo el mouse
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if (hit == null)
        {
            SetCursor(cursorDefault);
            return;
        }

        bool hayAldeano = SeleccionadorDeUnidad.Instance.unidadesSeleccionadas
            .Exists(u => u.GetComponent<Aldeano>() != null);

        if (hayAldeano)
        {
            if (hit.GetComponent<Tree>() != null)
                SetCursor(cursorHacha);
            else if (hit.GetComponent<Oro>() != null)
                SetCursor(cursorPico);
            else if (hit.GetComponent<Sheep>() != null)
                SetCursor(cursorPastor);
            else if ((hit.GetComponent<IAtacable>() != null && EsEnemigo(hit.gameObject)) ||
                     (hit.GetComponent<EdificioBase>() != null && EsEnemigo(hit.gameObject)))
                SetCursor(cursorEspada);
            else
                SetCursor(cursorDefault);
        }
        else
        {
            if ((hit.GetComponent<IAtacable>() != null && EsEnemigo(hit.gameObject)) ||
                (hit.GetComponent<EdificioBase>() != null && EsEnemigo(hit.gameObject)))
                SetCursor(cursorEspada);
            else
                SetCursor(cursorDefault);
        }
    }

    private void SetCursor(Texture2D tex)
    {
        if (tex == null) return;

        //  hotspot centrado
        Vector2 hotspot = new Vector2(tex.width / 2f, tex.height / 2f);

        Cursor.SetCursor(tex, hotspot, CursorMode.Auto);
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
