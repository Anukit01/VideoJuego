using System.Collections;
using UnityEngine;
using TMPro;

public class MensajeSistema : MonoBehaviour
{
    public GameObject panelMensaje;
    public TMP_Text textoMensaje;
    public float duracion = 2f;

    public void MostrarMensaje(string mensaje)
    {
        textoMensaje.text = mensaje;
        panelMensaje.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(OcultarMensaje());
    }

    IEnumerator OcultarMensaje()
    {
        yield return new WaitForSecondsRealtime(duracion);
        panelMensaje.SetActive(false);
    }
}
