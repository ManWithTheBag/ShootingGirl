using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakingDamage : AbsTakingDamage
{
    private AbsPlayerBaseModetState _currentPlayerModelState;

    private void OnEnable()
    {
        GlobalEventManager.ChangePlayerModelStateEvent.AddListener(SetCurrentPlayerModelState);
    }
    private void OnDisable()
    {
        GlobalEventManager.ChangePlayerModelStateEvent.RemoveListener(SetCurrentPlayerModelState);
    }

    private void SetCurrentPlayerModelState(PlayerModelEnum playerModelEnum)
    {
        _currentPlayerModelState = PlayerModelSwitcher.s_playerModelStateDictionary[playerModelEnum];
        _currentHealth = _currentPlayerModelState.currentHealthThisModel;

        StopAllCoroutines();
        _absCharacterUiControler.SetCurrentHealth(_currentHealth);
    }


    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        SaveCurrentHealthThisModel(_currentHealth);
    }

    private void SaveCurrentHealthThisModel(float currentHealth)
    {
        _currentPlayerModelState.SetCurrentHealthForThisModel(currentHealth);
    }
}
