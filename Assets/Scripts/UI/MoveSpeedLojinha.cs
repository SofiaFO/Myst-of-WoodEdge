using UnityEngine;
using TMPro;

public class MoveSpeedLojinha : MonoBehaviour
{
    [SerializeField] private TMP_Text tMP_Text;

    public void AtualizarMoveSpeed(float speed)
    {
        tMP_Text.text = "Velocidade: " + speed.ToString();
    }
}