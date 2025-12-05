using UnityEngine;
using TMPro;

public class AttackSpeedLojinha : MonoBehaviour
{
    [SerializeField] private TMP_Text tMP_Text;

    public void AtualizarAttackSpeed(float atkSpeed)
    {
        tMP_Text.text = "Attack Speed: " + atkSpeed.ToString();
    }
}