using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GestorVictoria : MonoBehaviour
{
    [SerializeField] private GameObject panelDecision;
    [SerializeField] private GameObject panelEstadisticas;
    [SerializeField] private GameObject panelVictoriaFinal;
    [SerializeField] private GameObject panelDerrota;
    [SerializeField] private TMP_Text textoEstadisticas;

    [SerializeField] private AudioSource fuenteVictoria;
    [SerializeField] private AudioClip clipVictoria;  
    [SerializeField] private AudioClip clipDerrota;

    //private int enemigosTotales = 0;
    //private int enemigosDerrotados = 0;
    public static GestorVictoria Instance { get; private set; }

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    

    public void SalirConEstilo()
    {
        panelDecision.SetActive(false);
        panelVictoriaFinal.SetActive(true);
        if (fuenteVictoria != null && clipVictoria != null)
        {
            fuenteVictoria.clip = clipVictoria;
            fuenteVictoria.Play();
        }
        Time.timeScale = 0f; // sigue pausado
    }

    public void SeguirLaMasacre()
    {
        panelDecision.SetActive(false);
        Time.timeScale = 1f;
    }

    public void VictoriaFinal()
    {     
        Time.timeScale = 0f;
      
        panelVictoriaFinal.SetActive(true);
        if (fuenteVictoria != null && clipVictoria != null)
        {
            fuenteVictoria.clip = clipVictoria;
            fuenteVictoria.Play();
        }
    }
    public void VerEstadisticas()
    {
        panelVictoriaFinal.SetActive(false);
        panelEstadisticas.SetActive(true);
        MostrarEstadisticas();
    }
    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); 
    }


    private void MostrarEstadisticas()
    {
        // Podés cargar los datos desde tu sistema actual
        int aldeanos = GestorEntidades.Instance.aldeanosCreados;
        int arqueros = GestorEntidades.Instance.arquerosCreados;
        int caballeros = GestorEntidades.Instance.caballerosCreados;



        TMP_Text texto = textoEstadisticas.GetComponent<TMP_Text>();
        texto.text = $"Has creado:\n{aldeanos} Aldeanos\n{arqueros} Arqueros\n{caballeros} Caballeros";
    }
    public void ActivarVictoriaParcial()
    {
        Time.timeScale = 0f; // Pausa el juego
        panelDecision.SetActive(true); // Muestra el mensaje de decisión
        
    }

    public void VerificarDerrota()
    {
        int bases = GestorEntidades.Instance.basesCreadas;
        int basesD = GestorEntidades.Instance.basesDestruidas;
        int aldeanos = GestorEntidades.Instance.aldeanosCreados;
        int aldeanosM = GestorEntidades.Instance.aldeanosMuertos;
        int recursosMadera = GestionRecrsos.Instance.madera;
        int recursosOro = GestionRecrsos.Instance.oro;
        int recursosAlimento = GestionRecrsos.Instance.alimento;

        // Condición 1: no hay bases ni aldeanos
        if (bases <= basesD && aldeanos <= aldeanosM )
        {
            ActivarDerrota();
            return;
        }

        // Condición 2: no hay bases y no hay recursos para construir una nueva
        if (bases <= basesD && !PuedeConstruirBase(recursosMadera, recursosOro, recursosAlimento))
        {
            ActivarDerrota();
            return;
        }

        // Condición 3: hay base, pero no aldeanos y no recursos para crear aldeanos
        if (aldeanos <= aldeanosM && !PuedeCrearAldeano(recursosAlimento, recursosOro))
        {
            ActivarDerrota();
            return;
        }
    }

    private bool PuedeConstruirBase(int madera, int oro, int alimento)
    {
       
        return madera >= 90 && oro >= 90 && alimento >= 40;
    }

    private bool PuedeCrearAldeano(int alimento, int madera)
    {
        
        return alimento >= 10 && madera >= 10;
    }

    private void ActivarDerrota()
    {
        Time.timeScale = 0f;
        panelDecision.SetActive(false);
        panelEstadisticas.SetActive(false);
        panelVictoriaFinal.SetActive(false);
        panelDerrota.SetActive(true);

        if (fuenteVictoria != null && clipVictoria != null)
        {
            fuenteVictoria.clip = clipVictoria;
            fuenteVictoria.Play();
        }

    }
}

