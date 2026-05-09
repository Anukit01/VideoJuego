using UnityEngine;

public static class FaccionUtils
{
    public static bool SonEnemigos(GameObject a, GameObject b)
    {
        if (a == null || b == null) return false;

        if (!a.TryGetComponent<EntidadBase>(out var entidadA)) return false;
        if (!b.TryGetComponent<EntidadBase>(out var entidadB)) return false;

        return entidadA.faccion != entidadB.faccion;
    }
    public static bool SonEnemigos(Faccion faccionA, Faccion faccionB)
    {
        return faccionA != faccionB;
    }

}
