using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    private PlayerData _playerData;
    private bool _isWin, _isLose;

    public UnityEvent<int> onUpdateCoins = new UnityEvent<int>();
    public UnityEvent<Vector3> onEnemyAlert = new UnityEvent<Vector3>();
    public UnityEvent onEnemyAlertOff = new UnityEvent();
    public UnityEvent onStart = new UnityEvent();
    public UnityEvent onPause = new UnityEvent();
    public UnityEvent onResume = new UnityEvent();
    public UnityEvent<int> onSelectItem = new UnityEvent<int>();
    public UnityEvent<int> onBuyItem = new UnityEvent<int>();
    public UnityEvent<bool> onEndGame = new UnityEvent<bool>();

    //EventManager
    public static UnityEvent<int> onOpenDoorEvent = new UnityEvent<int>();
    public static UnityEvent onPickUpKeyEvent = new UnityEvent();  
    public static UnityEvent<int> onEndEvent = new UnityEvent<int>();
    public static UnityEvent<int> onTeleportEvent = new UnityEvent<int>();
    public static UnityEvent<int> onLaserEvent = new UnityEvent<int>();

    public static UnityEvent<int> onPickPoisonEvent =  new UnityEvent<int>();

    protected virtual void Awake ()
    {
        base.Awake();
        _playerData = PlayerData.LoadData();
    }

    void Start ()
    {
        InitGame();
    }

    public void InitGame ()
    {
        _isLose = false;
        _playerData = PlayerData.LoadData();
        onUpdateCoins?.Invoke(_playerData.currentCoins);
    }

    public void StartGame()
    {
        onStart?.Invoke();
    }

    public void PauseGame()
    {
        onPause?.Invoke();
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        onResume?.Invoke();
        Time.timeScale = 1;
    }

    public void EndGame(bool win)
    {
        _isWin = win;
        onEndGame?.Invoke(_isWin);
        onUpdateCoins?.Invoke(_playerData.currentCoins);
        _playerData.SaveData();
    }

    public void UpdateCurrency(int coints)
    {
        _playerData.currentCoins += coints;
        onUpdateCoins?.Invoke(_playerData.currentCoins);
    }

    public bool BuyItem(int id, int price, TypeItem typeItem)
    {
        if (_playerData.currentCoins >= price)
        {
            switch (typeItem)
            {
                case TypeItem.SKIN:
                    if (_playerData.totalSkins.IndexOf(id) == -1)
                    {
                        _playerData.totalSkins.Add(id);
                        _playerData.SaveData();
                        onBuyItem?.Invoke(id);
                        return true;
                    }    
                    break;
                case TypeItem.HAT:
                    return true;
                    break;
                case TypeItem.SHIRT:
                    return true;
                    break;
                case TypeItem.PANT:
                    return true;
                    break;
                case TypeItem.SHOES:
                    return true;
                    break;
                default:
                    return true;
            }
        }
        return false;
    }

    public bool SelectItems(int id, TypeItem typeItem)
    {
        switch (typeItem)
        {
            case TypeItem.SKIN:
                return true;
                break;
            case TypeItem.HAT:
                return true;
                break;
            case TypeItem.SHIRT:
                return true;
                break;
            case TypeItem.PANT:
                return true;
                break;
            case TypeItem.SHOES:
                return true;
                break;
            default:
            return false;
        }
    }

    //EventManager
    public static void TeleportPlayer(int id)
    {
        onTeleportEvent?.Invoke(id);
    }

    public static void LaserOff(int id)
    {
        onLaserEvent?.Invoke(id);
    }

    public static void PickedUpKey()
    {
        onPickUpKeyEvent?.Invoke();
    }

    public static void StartDoorEvent(int id)
    {
        onOpenDoorEvent?.Invoke(id);
    }

    public static void EndGame(int id)
    {
        onEndEvent?.Invoke(id);
    }

    public static void InvisiblePoison(int id)
    {
        onPickPoisonEvent?.Invoke(id);
    }


}
