using UnityEngine;
using TMPro;


public class MultiplicadorLojinha : MonoBehaviour
{
    [SerializeField] private TMP_Text tMP_Text;
    public void AtualizarMult(float mult)
    {
        tMP_Text.text ="Multiplicador: " + mult.ToString();
    }
}
