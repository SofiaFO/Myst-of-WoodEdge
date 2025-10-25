using UnityEngine;
using TMPro;


public class DefesaLojinha : MonoBehaviour
{
    [SerializeField] private TMP_Text tMP_Text;
    public void AtualizarDefesa(float def)
    {
        tMP_Text.text ="Defesa: " + def.ToString();
    }
}
