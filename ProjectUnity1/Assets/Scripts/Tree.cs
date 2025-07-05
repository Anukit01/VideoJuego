using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour, IRecolectable
{
    public int maderaDisponible = 50;
    [SerializeField] private GameObject troncoVisual;   
    [SerializeField] private GameObject hojasVisual;    
    public TipoRecurso Tipo => TipoRecurso.Madera;
    public Transform PuntoDeRecoleccion => transform.Find("PuntoDeTala(0)");

    public IEnumerator EjecutarRecoleccion(Aldeano aldeano)
    {
        while (!aldeano.CargaRecoleccion.EstaLleno && maderaDisponible > 0)
        {
            yield return new WaitForSeconds(1f); // tiempo de corte por golpe
            maderaDisponible--;
            aldeano.CargaRecoleccion.cantidad++;
        }

        aldeano.TerminarRecoleccion();

        if (maderaDisponible <= 0)
            ConvertirseEnTronco();
    }
   

    private void ConvertirseEnTronco()
    {
        if (troncoVisual != null) troncoVisual.SetActive(true);
        if (hojasVisual != null) hojasVisual.SetActive(false);
        this.enabled = false; 
    }

}