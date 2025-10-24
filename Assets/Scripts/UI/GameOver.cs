using System;
using UnityEngine;
using UnityEngine.SceneManagement; 

namespace UI
{
    public class GameOver : MonoBehaviour
    {
        public void Exit()
        {
            Debug.Log("[MainUIController] Exit() chamado!");
            SceneManager.LoadScene("MainMenu");
        }
    }
}