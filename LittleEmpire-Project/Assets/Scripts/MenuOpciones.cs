using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuOpciones : MonoBehaviour
{
    [SerializeField] private GameObject panelOpciones;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            panelOpciones.SetActive(!panelOpciones.activeSelf);
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("Menu");
        panelOpciones.SetActive(!panelOpciones.activeSelf);
    }

    public void SalirDelJuego()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
