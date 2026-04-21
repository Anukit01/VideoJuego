using System.Collections.Generic;
using UnityEngine;

public class TutorialMan2 : MonoBehaviour
{
    public List<GameObject> panelesSlides; // lista de paneles prefabs
    private int indexActual = 0;
 
    public void IniciarTutorial()
    {
        Time.timeScale = 0;          // Pausa el juego
        indexActual = 0;             // Reinicia el índice
        MostrarSlide(indexActual);   // Muestra el primer panel
    }

    public void SiguienteSlide()
    {
        indexActual++;
        if (indexActual < panelesSlides.Count)
        {
            MostrarSlide(indexActual);
        }
        else
        {
            CerrarTutorial();
        }
    }

    public void SlideAnterior()
    {
        indexActual--;
        if (indexActual >= 0)
        {
            MostrarSlide(indexActual);
        }
        else
        {
            indexActual = 0;
        }
    }

    public void SaltarTutorial()
    {
        CerrarTutorial();
    }

    void MostrarSlide(int index)
    {
        
        foreach (var panel in panelesSlides)
            panel.SetActive(false);

        
        panelesSlides[index].SetActive(true);
    }

    void CerrarTutorial()
    {
        foreach (var panel in panelesSlides)
            panel.SetActive(false);

        Time.timeScale = 1;
    }
}