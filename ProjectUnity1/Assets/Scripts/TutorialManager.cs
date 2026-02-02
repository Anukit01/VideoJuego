using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TutorialSlide
{
    public string texto;
    public Sprite imagen;
}

public class TutorialManager : MonoBehaviour
{
    public TypewriterEffect typewriterEffect;
    public GameObject panelTutorial;
    public Image imagenTutorial;
    public TMP_Text textoTutorial;
    public List<TutorialSlide> slides;
    private int indexActual = 0;

    void Start()
    {
        Time.timeScale = 0;
        panelTutorial.SetActive(true);
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
            panelTutorial.SetActive(false);
            Time.timeScale = 1;
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
        panelTutorial.SetActive(false);
        Time.timeScale = 1;
    }

    void MostrarSlide(int index)
    {
        var slide = slides[index];
        typewriterEffect.MostrarTexto(slide.texto);


        if (slide.imagen != null)
        {
            imagenTutorial.gameObject.SetActive(true);
            imagenTutorial.sprite = slide.imagen;
        }
        else
        {
            imagenTutorial.gameObject.SetActive(false);
        }
    }

}