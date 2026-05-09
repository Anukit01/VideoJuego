using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TutorialSlide
{
    public string texto;
  
}

public class TutorialManager : MonoBehaviour
{
    public TypewriterEffect typewriterEffect;
    public GameObject panelHistoria;
    
    public TMP_Text textoTutorial;
    public List<TutorialSlide> slides;
    private int indexActual = 0;
    public TutorialMan2 tutorialManager;

    void Start()
    {
        Time.timeScale = 0;
        panelHistoria.SetActive(true);
        MostrarSlide(indexActual);
    }

    public void SiguienteSlide()
    {
        indexActual++;
        if (indexActual < slides.Count)
        {
            MostrarSlide(indexActual);
        }
        else
        {
            panelHistoria.SetActive(false);
            Time.timeScale = 1;
            tutorialManager.IniciarTutorial();
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
    public void SaltarHistoria()
    {
        panelHistoria.SetActive(false);
        Time.timeScale = 1;
        tutorialManager.IniciarTutorial();
    }

    void MostrarSlide(int index)
    {
        var slide = slides[index];
        typewriterEffect.MostrarTexto(slide.texto);        
    }

}