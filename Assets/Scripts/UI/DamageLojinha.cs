using UnityEngine;
using TMPro;

public class DamageLojinha : MonoBehaviour
{
    [SerializeField] private TMP_Text tMP_Text;

    public void AtualizarDano(float dano, bool isMax = false)
    {
        string maxText = isMax ? " [MAX]" : "";
        tMP_Text.text = "Dano: +" + dano.ToString("F0") + maxText;
    }
}