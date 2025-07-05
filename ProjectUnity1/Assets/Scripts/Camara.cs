using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Camara : MonoBehaviour
{
        public float velocidad = 5f;
        public int borde = 10;

    [SerializeField] private Tilemap mapaBase;
    private Vector2 limiteInferior;
    private Vector2 limiteSuperior;
    private Camera camara;

    void Start()
    {
        camara = Camera.main;

        if (mapaBase != null)
        {
            BoundsInt celdas = mapaBase.cellBounds;
            Vector3 min = mapaBase.CellToWorld(celdas.min);
            Vector3 max = mapaBase.CellToWorld(celdas.max);

            limiteInferior = min;
            limiteSuperior = max;
        }
    }


    void Update()
        {
            Vector3 movimiento = Vector3.zero;
            Vector3 mousePos = Input.mousePosition;

            if (mousePos.x < borde)
                movimiento.x = -1;
            else if (mousePos.x > Screen.width - borde)
                movimiento.x = 1;

            if (mousePos.y < borde)
                movimiento.y = -1;
            else if (mousePos.y > Screen.height - borde)
                movimiento.y = 1;

            transform.position += movimiento * velocidad * Time.deltaTime;
        float vertExtent = camara.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, limiteInferior.x + horzExtent, limiteSuperior.x - horzExtent);
        pos.y = Mathf.Clamp(pos.y, limiteInferior.y + vertExtent, limiteSuperior.y - vertExtent);

        transform.position = pos;

    }

}
