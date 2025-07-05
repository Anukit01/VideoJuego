using UnityEngine;

public class AvisarCambioVisual : MonoBehaviour
{
    private bool yaEjecutado = false;

    void OnEnable()
    {
        if (yaEjecutado || GestorOrdenVisualCamara.Instance == null)
            return;

        GestorOrdenVisualCamara.Instance.ActualizarOrdenes();
        yaEjecutado = true;
    }
}