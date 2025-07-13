using UnityEngine;

public class GestorSonido : MonoBehaviour
{
    public static GestorSonido instancia;

    [Header("Audios")]
    public AudioClip[] sonidoTala;
    public AudioClip[] sonidoConstruir;
    public AudioClip[] sonidoSheep;
    public AudioClip[] sonidoSheepMorir;

    private AudioSource fuente;

    private void Awake()
    {
        if (instancia == null) instancia = this;
        else Destroy(gameObject);

        fuente = GetComponent<AudioSource>();
    }

    public void ReproducirSonidoAccion(TipoAccion accion)
    {
        AudioClip clip = ObtenerClipAleatorio(accion);
        if (clip != null)
            fuente.PlayOneShot(clip);
    }

    private AudioClip ObtenerClipAleatorio(TipoAccion accion)
    {
        AudioClip[] lista = accion switch
        {
            TipoAccion.ConstruirS => sonidoConstruir,
            TipoAccion.TalarS => sonidoTala,
            TipoAccion.SheepS => sonidoSheep,
            TipoAccion.SheepSmorir => sonidoSheepMorir,

            _ => null
        };

        return lista != null && lista.Length > 0 ? lista[Random.Range(0, lista.Length)] : null;
    }
}

public enum TipoAccion { ConstruirS, TalarS, SheepS, SheepSmorir }