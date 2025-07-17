using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GestorVictoria : MonoBehaviour
{
    [SerializeField] private GameObject panelDecision;
    [SerializeField] private GameObject panelEstadisticas;
    [SerializeField] private GameObject panelVictoriaFinal;
    [SerializeField] private TMP_Text textoEstadisticas;

    //private int enemigosTotales = 0;
    //private int enemigosDerrotados = 0;
    public static GestorVictoria Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    //public void RegistrarEnemigo() => enemigosTotales++;
    //public void EnemigoMuerto()
    //{
    //    enemigosDerrotados++;

    //    float porcentaje = (float)enemigosDerrotados / enemigosTotales;

    //    if (porcentaje >= 0.75f)
    //    {
    //        Time.timeScale = 0f; // pausa el juego
    //        panelDecision.SetActive(true);
    //    }
    //}

    public void SalirConEstilo()
    {
        panelDecision.SetActive(false);
        panelVictoriaFinal.SetActive(true);    
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
        SceneManager.LoadScene("Menu"); // Asegurate de tenerlo en Build Settings
    }


    private void MostrarEstadisticas()
    {
        // Podés cargar los datos desde tu sistema actual
        int aldeanos = GestorEntidades.Instance.Contar("Aldeano");
        int arqueros = GestorEntidades.Instance.Contar("Arquero");
        int caballeros = GestorEntidades.Instance.Contar("Caballero");
        

        TMP_Text texto = panelEstadisticas.GetComponentInChildren<TMP_Text>();
        texto.text = $"Has creado:\n{aldeanos} Aldeanos\n{arqueros} Arqueros\n{caballeros} Caballeros";
    }
    public void ActivarVictoriaParcial()
    {
        Time.timeScale = 0f; // Pausa el juego
        panelDecision.SetActive(true); // Muestra el mensaje de decisión
        
    }
}

