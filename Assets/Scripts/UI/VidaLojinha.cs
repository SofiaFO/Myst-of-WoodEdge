using UnityEngine;
using TMPro;

public class VidaLojinha : MonoBehaviour
{
    [SerializeField] private TMP_Text tMP_Text;

    public void AtualizarVida(float vida, bool isMax = false)
    {
        print("Atualizando vida na lojinha para: " + vida);
        string maxText = isMax ? " [MAX]" : "";
        tMP_Text.text = "Vida: " + vida.ToString("F0") + maxText;
    }
}