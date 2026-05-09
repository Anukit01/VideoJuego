using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScene : MonoBehaviour
{
    [SerializeField] private GameObject panelOpciones;
   
    private void Awake()
    {
        // Asegurarse de que este objeto no se destruya al cambiar de escena
        DontDestroyOnLoad(gameObject);
       
    }
    public void CambiarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    public void MostrarOpciones()
    {
        panelOpciones.SetActive(true);
    }




}
