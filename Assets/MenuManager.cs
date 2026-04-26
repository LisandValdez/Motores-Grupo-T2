using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject panelInstrucciones;

    public void MostrarInstrucciones()
    {
        panelInstrucciones.SetActive(true);
    }

    public void CerrarInstrucciones()
    {
        panelInstrucciones.SetActive(false);
    }
}