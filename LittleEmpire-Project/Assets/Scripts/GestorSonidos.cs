using UnityEngine;

public class GestorSonido : MonoBehaviour
{
    public static GestorSonido instancia;

    [Header("Audios")]
    public AudioClip[] sonidoFondo;

    private AudioSource fuente;

    private void Awake()
    {
        if (instancia == null) instancia = this;
        else Destroy(gameObject);

        fuente = GetComponent<AudioSource>();
    }
   
}
