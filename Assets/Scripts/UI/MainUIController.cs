using System;
using UnityEngine;
using UnityEngine.SceneManagement; 

namespace UI
{
    public class MainUIController : MonoBehaviour
    {
        public void StartGame()
        {
            Debug.Log("[MainUIController] StartGame() chamado!");
            SceneManager.LoadScene("PrefabScene");
        }
        
        public void Loja()
        {
            Debug.Log("[MainUIController] Loja() chamado!");
            SceneManager.LoadScene("Lojinha");
        }
        
        public void Exit()
        {
            Debug.Log("[MainUIController] Exit() chamado!");
            Application.Quit();
        }
    }
}