using UnityEngine;

public class SelectorPorArrastre : MonoBehaviour
{
    [SerializeField] private RectTransform boxVisual;

    private Vector2 startPosition;
    private Vector2 endPosition;
    private Rect selectionBox;
    private bool arrastrando;

    public bool EstaArrastrando => arrastrando;

    void Start()
    {
        boxVisual.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

            arrastrando = true;
            startPosition = Input.mousePosition;
            selectionBox = new Rect();
            boxVisual.gameObject.SetActive(true);
        }

        if (arrastrando)
        {
            endPosition = Input.mousePosition;
            DibujarVisual();
            CalcularRectangulo();

            if (Input.GetMouseButtonUp(0))
            {
                arrastrando = false;
                SeleccionarUnidades();
                startPosition = endPosition = Vector2.zero;
                boxVisual.sizeDelta = Vector2.zero;
                boxVisual.gameObject.SetActive(false);
            }
        }
    }

    void DibujarVisual()
    {
        Vector2 centro = (startPosition + endPosition) / 2f;
        Vector2 tamaño = new Vector2(Mathf.Abs(startPosition.x - endPosition.x), Mathf.Abs(startPosition.y - endPosition.y));

        boxVisual.position = centro;
        boxVisual.sizeDelta = tamaño;
    }

    void CalcularRectangulo()
    {
        selectionBox.xMin = Mathf.Min(startPosition.x, endPosition.x);
        selectionBox.xMax = Mathf.Max(startPosition.x, endPosition.x);
        selectionBox.yMin = Mathf.Min(startPosition.y, endPosition.y);
        selectionBox.yMax = Mathf.Max(startPosition.y, endPosition.y);
    }

    void SeleccionarUnidades()
    {
        foreach (GameObject unidad in SeleccionadorDeUnidad.Instance.todasLasUnidades)
        {
            if (unidad == null) continue;
            if (unidad.TryGetComponent<Aldeano>(out var aldeano) && aldeano.EstaOcupadoPrivado)
                continue; // no seleccionarlo
            Vector2 pantalla = Camera.main.WorldToScreenPoint(unidad.transform.position);
            if (selectionBox.Contains(pantalla))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    SeleccionadorDeUnidad.Instance.AlternarSeleccion(unidad);
                else
                    SeleccionadorDeUnidad.Instance.SeleccionDrag(unidad);
            }
        }
    }
}