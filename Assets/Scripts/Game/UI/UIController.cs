using Components;
using Game.Configs;
using Services;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

namespace Game.UI
{
    public class UIController : MonoBehaviour 
    {
        [SerializeField] private GameConfig config;
        
        [Header("Game UI")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI turnParticipantText;
        [SerializeField] private Button menuButton;
        
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

        private TurnService _turnService;
        
        public void Init(TurnService turnService)
        {
            _turnService = turnService;
        }
        
        private void OnValidate()
        {
            _playerScoreTextColor = config.playerMaterial.color.ToHexString();
            _aiScoreTextColor = config.aiMaterial.color.ToHexString();

            UpdateText();
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
            UpdateText();
        }

        private void UpdateText()
        {
            scoreText.text = $"<color=#{_playerScoreTextColor}>{ScoreService.Player}</color>:" +
                             $"<color=#{_aiScoreTextColor}>{ScoreService.AI}</color>";
            
            var participant = _turnService?.CurrentTurn().Participant ?? TurnParticipant.Player;
            var color = participant == TurnParticipant.Player ? _playerScoreTextColor : _aiScoreTextColor;
            var text = participant == TurnParticipant.Player ? "You" : "AI";

            turnParticipantText.text = $"Current turn:\n<color=#{color}>{text}</color>";
        }

        public void ShowResult(TurnParticipant participant) 
        {
            var color = participant == TurnParticipant.Player ? _playerScoreTextColor : _aiScoreTextColor;
            var text = participant == TurnParticipant.Player ? "You Win" : "You Lose";
            resultText.text = $"\n<color=#{color}>{text}</color>";
                
            mainMenuPanel.SetActive(false);
            endGameMenuPanel.SetActive(true);
            _currentMenuPanel = endGameMenuPanel;
            
            GameStateController.PauseGame(true);
        }
        
        private void OnRestart() 
        {
            GameStateController.Restart();
        }

        void OnMenuButton()
        {
            bool show = !_currentMenuPanel.activeSelf;
            
            _currentMenuPanel.SetActive(!_currentMenuPanel.activeSelf);
            GameStateController.PauseGame(show);
        }
        
        void OnSave() 
        {
            GameStateController.Save();
        }
        
        void OnLoad() 
        {
            GameStateController.Load();
        }
        
        private void OnQuit() 
        {
            Application.Quit();
        }
    }

}