using System;
using Components;
using Game.Configs;
using Services;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;

namespace Game.UI
{
    public class UIController : MonoBehaviour 
    {
        [Header("Game UI")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Button menuButton;
        [SerializeField] private GameConfig config;
        
        [Header("Main Menu Panel")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button quitButton;
        
        [Header("End Game Menu Panel")]
        [SerializeField] private GameObject endGameMenuPanel;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private Button endRestartButton;
        [SerializeField] private Button endLoadButton;
        [SerializeField] private Button endQuitButton;

        private GameObject _currentMenuPanel;

        private string _playerScoreTextColor;
        private string _aiScoreTextColor;

        private void OnValidate()
        {
            _playerScoreTextColor = "#" + config.playerMaterial.color.ToHexString();
            _aiScoreTextColor = "#" + config.aiMaterial.color.ToHexString();

            UpdateScoreText();
        }

        private void Start() 
        {
            restartButton.onClick.AddListener(OnRestart);
            saveButton.onClick.AddListener(OnSave);
            loadButton.onClick.AddListener(OnLoad);
            quitButton.onClick.AddListener(OnQuit);

            endRestartButton.onClick.AddListener(OnRestart);
            endLoadButton.onClick.AddListener(OnLoad);
            endQuitButton.onClick.AddListener(OnQuit);

            mainMenuPanel.SetActive(false);
            endGameMenuPanel.SetActive(false);

            menuButton.onClick.AddListener(OnMenuButton);

            _currentMenuPanel = mainMenuPanel;
        }

        private void OnDestroy()
        {
            restartButton.onClick.RemoveListener(OnRestart);
            saveButton.onClick.RemoveListener(OnSave);
            loadButton.onClick.RemoveListener(OnLoad);
            quitButton.onClick.RemoveListener(OnQuit);

            endRestartButton.onClick.RemoveListener(OnRestart);
            endLoadButton.onClick.RemoveListener(OnLoad);
            endQuitButton.onClick.RemoveListener(OnQuit);

            menuButton.onClick.RemoveListener(OnMenuButton);
        }

        private void Update()
        {
            UpdateScoreText();
        }

        private void UpdateScoreText()
        {
            scoreText.text = $"<color={_playerScoreTextColor}>{ScoreService.Player}</color>:" +
                             $"<color={_aiScoreTextColor}>{ScoreService.AI}</color>";
        }

        public void ShowResult(bool win) {
            resultText.text = win ? "You Win" : "You Lose";
            
            mainMenuPanel.SetActive(false);
            endGameMenuPanel.SetActive(true);
            _currentMenuPanel = endGameMenuPanel;
        }
        
        private void OnRestart() {
            GameStateController.Restart();
        }

        void OnMenuButton(){
            _currentMenuPanel.SetActive(!_currentMenuPanel.activeSelf);
        }
        
        void OnSave() {
            GameStateController.Save();
        }
        void OnLoad() {
            GameStateController.Load();
        }
        
        private void OnQuit() {
            Application.Quit();
        }
    }

}