

using UnityEngine;

public interface IAtacable
{
    void RecibirDanio(int cantidad, GameObject atacante);

    bool EstaVivo();
}

