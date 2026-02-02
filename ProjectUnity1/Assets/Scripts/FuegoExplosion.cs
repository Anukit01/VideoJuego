using System.Collections;
using UnityEngine;

public class ExplosionFuego : MonoBehaviour
{
    private int danio;
    private GameObject emisor;
    private float radio;
    private bool yaAplicado = false;

    public void Configurar(int valorDanio, GameObject origen, float radioExplosion)
    {
        danio = valorDanio;
        emisor = origen;
        radio = radioExplosion;

        StartCoroutine(AplicarDanioEnArea());
    }

    private IEnumerator AplicarDanioEnArea()
    {
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radio);
    }
}