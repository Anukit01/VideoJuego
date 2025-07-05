using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GestorVisualInicial : MonoBehaviour
{
    [SerializeField] private GameObject pantallaCarga;
    [SerializeField] private Text textoCarga;

    void Start()
    {
        StartCoroutine(OrganizarMundo());
    }

    private IEnumerator OrganizarMundo()
    {
        pantallaCarga.SetActive(true);
        textoCarga.text = "Organizando el mundo...";

        yield return null; // Esperar un frame para que todo est� instanciado

        GestorOrdenVisualCamara.Instance?.ActualizarOrdenes();

        yield return new WaitForSeconds(0.5f); // Simular tiempo de organizaci�n

        textoCarga.text = "�Listo para jugar!";
        yield return new WaitForSeconds(0.5f);

        pantallaCarga.SetActive(false);
    }
}