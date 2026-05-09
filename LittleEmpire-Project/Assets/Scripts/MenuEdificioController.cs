using UnityEngine;
using UnityEngine.EventSystems;

public class MenuEdificioController : MonoBehaviour
{
    public GameObject menuEdificio;

    void Update()
    {
        if (menuEdificio != null && menuEdificio.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Raycast contra el mundo
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                // Si no tocamos la base ni el menú, cerramos
                if (hit.collider == null ||
                    (hit.collider.gameObject != gameObject && hit.collider.gameObject != menuEdificio))
                {
                    menuEdificio.SetActive(false);
                }
            }
        }
    }
}