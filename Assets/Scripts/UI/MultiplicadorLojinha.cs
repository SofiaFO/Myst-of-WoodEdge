using UnityEngine;
using TMPro;

public class MultiplicadorLojinha : MonoBehaviour
{
    [SerializeField] private TMP_Text tMP_Text;

    public void AtualizarMult(float mult, bool isMax = false)
    {
        print("Atualizando multiplicador na lojinha para: " + mult);
        string maxText = isMax ? " [MAX]" : "";
        tMP_Text.text = "Multiplicador: " + mult.ToString("F1") + "x" + maxText;
        print("Texto atualizado para: " + tMP_Text.text);
    }
}