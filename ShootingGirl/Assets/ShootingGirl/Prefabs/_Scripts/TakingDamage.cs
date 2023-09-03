using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakingDamage : MonoBehaviour
{
    [SerializeField] private CommonGameInfo _commonGameInfo;

    private GameObject _uiController;
    private AbsCharacter _thisCharacter;
    private ScoreController _scoreController;
    private AbsCharacterUiController _absCharacterUiControler;
    private float _currentHealth;
    private LerpValue _lerpValue;

    public float currentHealth { get { return _currentHealth; } private set { _currentHealth = value; } }

    private void Awake()
    {
        _thisCharacter = GetComponent<AbsCharacter>();
        _uiController = GameObject.Find("UIController");
        _scoreController = _uiController.GetComponent<ScoreController>();
        _absCharacterUiControler = GetComponent<AbsCharacterUiController>();
        _currentHealth = _thisCharacter.characterInfo.fullHealth;
        _lerpValue = new LerpValue(_commonGameInfo.timeLerpingValue);
    }

    private void OnEnable()
    {
        _lerpValue.LerpChengedValueEvent += GetLerpResult;
    }
    private void OnDisable()
    {
        _lerpValue.LerpChengedValueEvent -= GetLerpResult;
    }

    public void TakeDamage(float damage)
    {
        float oldHealthValue = _currentHealth;
        _currentHealth -= damage;
        
        SetLerpValue(oldHealthValue, _currentHealth);

        if (_currentHealth <= 0)
        {
            DeathCharacter();
        }
    }

    private void SetLerpValue(float oldScoreValue, float newScoreValue)
    {
        _lerpValue.SetNewValue(oldScoreValue, newScoreValue);
    }
    private void GetLerpResult(float result)
    {
        _absCharacterUiControler.SetCurrentHealth(result);
    }


    private void DeathCharacter()
    {
        _thisCharacter.TotalRefresh();
        _scoreController.AddScore(_thisCharacter.characterInfo.scoreInEnemy);
        _currentHealth = _thisCharacter.characterInfo.fullHealth;
    }
}
