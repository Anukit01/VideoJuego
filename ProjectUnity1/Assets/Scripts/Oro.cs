using System.Collections;
using UnityEngine;

public class Oro : MonoBehaviour, IRecolectable
{
    public int cantidad = 90;
    public GameObject oroRecolectadoPrefab;
    [SerializeField] private GameObject visualInactiva;
    [SerializeField] private GameObject visualActiva;
    [SerializeField] private GameObject visualDestruida;
    [SerializeField] private Transform puntoDeRecoleccion;
    public Transform PuntoDeRecoleccion => puntoDeRecoleccion;

    [SerializeField] private AudioSource fuenteEdificio;
    [SerializeField] private AudioClip clipDerrumbarse;
    

    public TipoRecurso Tipo => TipoRecurso.Oro;

    public IEnumerator EjecutarRecoleccion(Aldeano aldeano)
    {
        yield return new WaitForSeconds(0.5f);
        Recolectar(aldeano);
        aldeano.TerminarRecoleccion();
    }


    public void Recolectar(Aldeano aldeano)
    {
        int cantidadExtraida = 5;

        if (cantidad <= 0) return;

        cantidad -= cantidadExtraida;
        MostrarVisualActiva();

        aldeano.CargaRecoleccion.visual = aldeano.VisualBolsaOro;
        aldeano.CargaRecoleccion.tipo = TipoRecurso.Oro;
        aldeano.CargaRecoleccion.cantidad = cantidadExtraida;


        if (cantidad <= 0)
            MostrarVisualDestruida();
    }

    public void RevertirVisualActiva()
    {
        visualActiva.SetActive(false);
        visualInactiva.SetActive(true);
       if (fuenteEdificio != null && fuenteEdificio.isPlaying)
        {
            fuenteEdificio.Stop();
        }
    }

    public void MostrarVisualActiva()
    {
        visualInactiva.SetActive(false);
        visualActiva.SetActive(true);      
    }

    public void MostrarVisualDestruida()
    {
        visualActiva.SetActive(false);
        visualDestruida.SetActive(true);
        if (fuenteEdificio != null && clipDerrumbarse != null)
        {
            ReproducirUna(clipDerrumbarse);
            // Podés hacer que el objeto ya no sea seleccionable
        }

        Destroy(gameObject, 4f); // Destruye el objeto después de 2 segundos
       
    }
    protected void OnDestroy()
    {
        BuildingPlacementManager.Instance?.ActualizarNavMesh();

    }
    public void ReproducirLoop(AudioClip clip)
    {
        if (fuenteEdificio == null) return;
        fuenteEdificio.clip = clip;
        fuenteEdificio.loop = true;
        fuenteEdificio.Play();
    }
    public void ReproducirUna(AudioClip clip)
    {
        if (fuenteEdificio == null || clip == null) return;
        fuenteEdificio.Stop();
        fuenteEdificio.clip = clip;
        fuenteEdificio.loop = false;
        fuenteEdificio.spatialBlend = 0.5f; // 2D sound
        fuenteEdificio.PlayOneShot(clip);
    }
}
