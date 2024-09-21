using System;
using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private ServerTimeProvider _serverTimeProvider;
    [SerializeField] private Controls _controls;
    [SerializeField] private ClockView _clockView;
    
    private DateTime _currentTime;

    public event Action<DateTime> Time—hanged;
    

    private void Start()
    {
        StartCoroutine(UpdateTimeRoutine());
    }

    private void OnEnable()
    {
        _serverTimeProvider.TimeIsLoaded += ChangeTime;

        _controls.ChangeModeEnabled += EnableChangeMode;

        _clockView.TimeManually—hanged += ChangeTimeManually;
    }

    private void OnDisable()
    {
        _serverTimeProvider.TimeIsLoaded -= ChangeTime;

        _controls.ChangeModeEnabled -= EnableChangeMode;

        _clockView.TimeManually—hanged -= ChangeTimeManually;
    }

    private void ChangeTime(DateTime dateTime)
    {
        _currentTime = dateTime;

        Time—hanged?.Invoke(_currentTime);
    }

    private IEnumerator UpdateTimeRoutine()
    {
        _currentTime = _currentTime.AddSeconds(1d);

        Time—hanged?.Invoke(_currentTime);

        yield return new WaitForSecondsRealtime(1f);

        yield return UpdateTimeRoutine();
    }

    private void EnableChangeMode(bool changeModeEnabled)
    {
        if (changeModeEnabled)
        {
            StopAllCoroutines();
        }
        else
        {
            StartCoroutine(UpdateTimeRoutine());
        }
    }

    private void ChangeTimeManually(DateTime dateTime)
    {
        _currentTime = dateTime;

        Time—hanged?.Invoke(_currentTime);
    }
}
