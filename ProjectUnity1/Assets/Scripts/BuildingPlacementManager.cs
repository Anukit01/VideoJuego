using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class BuildingPlacementManager : MonoBehaviour
{
    public static BuildingPlacementManager Instance;

    [SerializeField] private NavMeshPlus.Components.NavMeshSurface navMeshSurface;

    private GameObject currentPrefab;        // Prefab real del edificio a construir
    private GameObject currentGhostPrefab;   // Prefab fantasma para previsualización
    private GameObject ghostInstance;        // Instancia en escena del fantasma

    private bool placing = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!placing || ghostInstance == null) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ghostInstance.transform.position = mousePos;

        bool valid = CheckValidPlacement(mousePos);
        SetGhostColor(valid ? new Color(0f, 1f, 0f, 0.85f) : new Color(1f, 0f, 0f, 0.85f));

        if (Input.GetMouseButtonDown(0) && valid)
        {
            placing = false;
            Destroy(ghostInstance);
            SendBuildOrder(mousePos);
        }
    }

    public bool IsPlacing() => placing;

    public void BeginPlacement(GameObject prefabReal, GameObject prefabFantasma)
    {
        currentPrefab = prefabReal;
        currentGhostPrefab = prefabFantasma;
        placing = true;

        if (ghostInstance != null)
            Destroy(ghostInstance);

        ghostInstance = Instantiate(currentGhostPrefab);
    }

    public void ActualizarNavMesh()
    {
        if (this != null)
        {
            StartCoroutine(RebuildNavmeshConDelay());
        }
        
    }

    private IEnumerator RebuildNavmeshConDelay()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();
        else
            Debug.LogWarning("NavMeshSurface no asignado.");
    }


    bool CheckValidPlacement(Vector2 pos)
    {
        Collider2D hit = Physics2D.OverlapCircle(pos, 0.5f);
        return hit == null || hit.gameObject.tag != "SortObject";
    }

    private void SetGhostColor(Color color)
    {
        foreach (var sr in ghostInstance.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = color;
        }
    }

    private void SendBuildOrder(Vector2 posicion)
    {
        GameObject aldeano = SeleccionadorDeUnidad.Instance.unidadesSeleccionadas
            .FirstOrDefault(u => u.TryGetComponent<Aldeano>(out _));

        if (aldeano != null)
        {
            aldeano.GetComponent<Aldeano>().OrdenarConstruccion(currentPrefab, posicion);
        }
    }
}