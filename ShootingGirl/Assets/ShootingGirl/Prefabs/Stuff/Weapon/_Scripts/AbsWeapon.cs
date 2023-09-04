using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsWeapon : MonoBehaviour
{
    [SerializeField] protected WeaponInfo _weaponInfo;
    [SerializeField] protected Transform _firePosition;

    protected AbsCharacter _absCharacter;
    protected ShellPoolContainer _shellPoolContainer;
    protected IPool<AbsShell> _absShell;
    protected Transform _aimShotTransform;
    protected Vector3 _aimDirection;

    //Shot timer condition
    private float _currentTimeRechargeBullet;
    private float _currentTimeCoolingOverheat;
    private int _currentCountBulletToOverheat;
    private float _currentTimeCoolingBulletRecharge;

    public WeaponInfo weaponInfo { get { return _weaponInfo; } private set { _weaponInfo = value; } }
    public Transform aimShotTransform { get { return _aimShotTransform; } private set { _aimShotTransform = value; } }

    public virtual void Awake()
    {
        _shellPoolContainer = GameObject.Find("ShellCotroller").GetComponent<ShellPoolContainer>();
        SetAbsCharacter();
        SetShellForWeapon();
    }
    private void Start()
    {
        SetAimSootTransform();
    }

    public abstract void SetAbsCharacter();
    public abstract void SetShellForWeapon();
    public abstract void SetAimSootTransform();

    #region Check Weapon Conditions like: Recharge and Overheat;

    private void Update()
    {
        ChechOverheat();
    }
    private void ChechOverheat()
    {
        if (_weaponInfo.isCanOverheat)
        {
            if (_currentCountBulletToOverheat <= _weaponInfo.countBulletToOverheat)
            {
                CalculateAimDirection();
                CheckShotWeaponCondition();
            }
            else
            {
                StartCoolingOverheat();
            }
        }
        else
        {
            CalculateAimDirection();
            CheckShotWeaponCondition();
        }
    }
    private void CheckShotWeaponCondition()
    {
        if (CheckTimeRechargeBullet() && CheckPersonalWeaponCondition())
        {
            Shot();

            _currentCountBulletToOverheat ++;
            SetLerpValue(_currentCountBulletToOverheat, _weaponInfo.countBulletToOverheat, false);
        }

        if (CheckTimeCoolingBulletRecharge() && CheckPersonalWeaponCondition() == false)
        {
            if (_currentCountBulletToOverheat > 0)
            {
                _currentCountBulletToOverheat --;
                SetLerpValue(_currentCountBulletToOverheat, _weaponInfo.countBulletToOverheat, false);
            }
        }
    }
    private void StartCoolingOverheat()
    {
        if (CheckTimeCoolingOverheat())
        {
            _currentCountBulletToOverheat = 0;
        }
        else
        {
            SetLerpValue(_currentTimeCoolingOverheat, _weaponInfo.timeCoolingOverheat, true);
        }
    }

    #endregion
    public abstract bool CheckPersonalWeaponCondition();

    #region Timer Region

    private bool CheckTimeRechargeBullet()
    {
        return DefaultTimer(ref _currentTimeRechargeBullet, _weaponInfo.timeRechargeBullet);
    }

    private bool CheckTimeCoolingBulletRecharge()
    {
        return DefaultTimer(ref _currentTimeCoolingBulletRecharge, _weaponInfo.timeCoolingBulletRecharge);
    }
    
    private bool CheckTimeCoolingOverheat()
    {
        return DefaultTimer(ref _currentTimeCoolingOverheat, _weaponInfo.timeCoolingOverheat);
    }

    private bool DefaultTimer(ref float currentValue, float defaultValue)
    {
        if (currentValue >= defaultValue)
        {
            currentValue = 0;
            return true;
        }
        else
        {
            currentValue += Time.deltaTime;
            return false;
        }
    }
    #endregion

    public virtual void SetLerpValue(float currentValue, float defaultValue, bool isRevers) { }

    private void CalculateAimDirection()
    {
        _aimDirection = (_aimShotTransform.position - _firePosition.position);
    }

    protected void Shot()
    {
        AbsShell shell = _absShell.GetElement();
        shell._thisTransform.position = _firePosition.position;
        shell._thisTransform.rotation = _firePosition.rotation;
        shell.SetShellData(_weaponInfo.damage, _absCharacter.thisTransform);
        shell.SetupShell(_aimDirection, _weaponInfo.sppedOfShell);
        shell.SetVisibleStatusGO(true);
    }

}
