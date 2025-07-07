using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VidaVisual : MonoBehaviour
{
    [SerializeField] private Image barraVida;
    [SerializeField] private EntidadBase entidad;
    [SerializeField] private GameObject canvasVida;

    [SerializeField] private TextMeshProUGUI textoVida;

    void Start()
    {
        if (entidad == null)
            entidad = GetComponentInParent<EntidadBase>();

        Invoke(nameof(ActualizarVidaVisual), 0.01f); // pequeño retraso
    }   

        public void Mostrar(bool mostrar)
        {
            if (canvasVida != null)
                canvasVida.SetActive(mostrar);
        }

        public void ActualizarVidaVisual()
    {
        if (barraVida == null || entidad == null) return;
        barraVida.fillAmount = (float)entidad.VidaActual / entidad.VidaMaxima;
        if (textoVida != null)
            textoVida.text = $"{entidad.VidaActual} / {entidad.VidaMaxima}";

    }
    

}