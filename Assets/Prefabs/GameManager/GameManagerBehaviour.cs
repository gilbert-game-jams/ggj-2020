using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManagerBehaviour : MonoBehaviour
{
    public TextMeshProUGUI _healthDisplay;
    public TextMeshProUGUI _timeDisplay;
    public TextMeshProUGUI _endOfGameMessage;
    public float _health = 1000;
    public float _penalty = 5f;
    public float _penaltyCooldown = 1f;
    public float _time = 300;
    CrackManager _crackManager;
    float _timeUntilPenalty;
    bool _gameIsOngoing = true;

    private void Awake() {
        _timeUntilPenalty = _penaltyCooldown;
    }

    private void Start() {
        _crackManager = FindObjectOfType<CrackManager>();
    }

    private void Update() {
        if(_health <= 0 || _time <= 0) {
            EndOfGame();
            _gameIsOngoing = false;
        }

        if (_gameIsOngoing) {
            _time -= Time.deltaTime;
            _timeUntilPenalty -= Time.deltaTime;
        
            if(_timeUntilPenalty <= 0) {
                _health -= _penalty * _crackManager.OpenCracks.Count;
                _timeUntilPenalty = _penaltyCooldown;
            }
        
            _healthDisplay.text = "Health: " + ((int) _health).ToString();
            _timeDisplay.text = "Time: " + ((int) _time).ToString();
        } else {
            if (Input.GetKeyDown(KeyCode.R)) {
                SceneManager.LoadScene( SceneManager.GetActiveScene().name );
            }
        }
    }

    void EndOfGame() {
        _endOfGameMessage.gameObject.SetActive(true);
        if(_health > 0 && _time <= 0) {
            _endOfGameMessage.text = WinMessage();
        } else {
            _endOfGameMessage.text = LoseMessage();
        }
    }

    string WinMessage() {
        return "You won! Press 'r' to play again";
    }

    string LoseMessage() {
        return "You lost! Press 'r' to play again";
    }
}
