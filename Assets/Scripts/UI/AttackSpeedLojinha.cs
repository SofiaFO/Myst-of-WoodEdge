using UnityEngine;
using TMPro;

public class AttackSpeedLojinha : MonoBehaviour
{
    [SerializeField] private TMP_Text tMP_Text;

    public void AtualizarAttackSpeed(float atkSpeed, bool isMax = false)
    {
        string maxText = isMax ? " [MAX]" : "";
        tMP_Text.text = "Attack Speed: +" + atkSpeed.ToString("F1") + maxText;
    }
}