using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    public float velocidad = 0.05f;
    public TMP_Text textoUI;

    public void MostrarTexto(string textoCompleto)
    {
        StopAllCoroutines(); // Por si ya se estaba mostrando texto
        StartCoroutine(AnimarTexto(textoCompleto));
    }

    IEnumerator AnimarTexto(string textoCompleto)
    {
        textoUI.text = "";
        foreach (char letra in textoCompleto)
        {
            textoUI.text += letra;
            yield return new WaitForSecondsRealtime(velocidad); // para funcionar con Time.timeScale = 0
        }
    }
}