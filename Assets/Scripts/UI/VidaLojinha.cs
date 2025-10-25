using UnityEngine;
using TMPro;


public class VidaLojinha : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TMP_Text tMP_Text;
    public void AtualizarVida(float vida)
    {
        tMP_Text.text ="Vida: " + vida.ToString();
    }
}
