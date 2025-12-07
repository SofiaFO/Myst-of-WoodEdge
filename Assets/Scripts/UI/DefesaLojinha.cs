using UnityEngine;
using TMPro;

public class DefesaLojinha : MonoBehaviour
{
    [SerializeField] private TMP_Text tMP_Text;

    public void AtualizarDefesa(float def, bool isMax = false)
    {
        string maxText = isMax ? " [MAX]" : "";
        tMP_Text.text = "Defesa: " + def.ToString("F0") + maxText;
    }
}