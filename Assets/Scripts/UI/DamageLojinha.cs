using UnityEngine;
using TMPro;

public class DamageLojinha : MonoBehaviour
{
    [SerializeField] private TMP_Text tMP_Text;

    public void AtualizarDano(float dano)
    {
        tMP_Text.text = "Dano: " + dano.ToString();
    }
}