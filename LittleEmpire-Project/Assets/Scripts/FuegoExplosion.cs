using System.Collections;
using UnityEngine;

public class ExplosionFuego : MonoBehaviour
{
    private int danio;
    private GameObject emisor;
    private float radio;
    private bool yaAplicado = false;
    [SerializeField] private AudioSource fuenteAudio;
    [SerializeField] private AudioClip clipFuego;
    public void Configurar(int valorDanio, GameObject origen, float radioExplosion)
    {
        danio = valorDanio;
        emisor = origen;
        radio = radioExplosion;

        StartCoroutine(AplicarDanioEnArea());
    }

    private IEnumerator AplicarDanioEnArea()
    {
        ReproducirUna(clipFuego);
        if (yaAplicado) yield break;
        yaAplicado = true;

        yield return new WaitForSeconds(0.15f); // sincronizar con animación si hay

        Collider2D[] afectados = Physics2D.OverlapCircleAll(transform.position, radio);
        foreach (var col in afectados)
        {
            if (col.TryGetComponent<Sheep>(out _)) continue;

            if (emisor.TryGetComponent<EntidadBase>(out var entidadEmisor) &&
                col.TryGetComponent<EntidadBase>(out var entidadObjetivo))
            {
                if (!FaccionUtils.SonEnemigos(entidadEmisor.faccion, entidadObjetivo.faccion))
                    continue;

                if (entidadObjetivo is IAtacable atacable)
                {
                    atacable.RecibirDanio(danio, emisor);
                }
            }
        }

        Destroy(gameObject);
    }
    public void ReproducirLoop(AudioClip clip)
    {
        if (fuenteAudio == null) return;
        fuenteAudio.clip = clip;
        fuenteAudio.loop = true;
        fuenteAudio.Play();
    }

    public void ReproducirUna(AudioClip clip)
    {
        if (fuenteAudio == null || clip == null) return;
        fuenteAudio.Stop();
        fuenteAudio.clip = clip;
        fuenteAudio.loop = false;
        fuenteAudio.spatialBlend = 0.5f;
        fuenteAudio.PlayOneShot(clip);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.5f); // rojo semi-transparente
        Gizmos.DrawWireSphere(transform.position, radio); // radio de explosión
    }
}