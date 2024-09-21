using UnityEngine;
using System;
using DG.Tweening;
using TMPro;
using System.Text.RegularExpressions;

public class ClockView : MonoBehaviour
{
    [SerializeField] private TimeManager _timeManager;
    [SerializeField] private Controls _controls;
    [SerializeField] private TMP_InputField _timeField;
    [SerializeField] private Transform _hoursArrow, _minutesArrow, _secondsArrow;

    private const float
        _hoursToDegrees = 360f / 12f,
        _minutesToDegrees = 360f / 60f,
        _secondsToDegrees = 360f / 60f;

    private int _previousHour;
    private DateTime _currentTime;
    private bool _changeModeEnabled;

    public event Action<DateTime> TimeManually—hanged;

    private void Start()
    {
        DragArrow[] dragArrows = FindObjectsOfType<DragArrow>();
        foreach (var dragArrow in dragArrows)
        {
            dragArrow.gameObject.GetComponent<DragArrow>().enabled = false;
        }

        _timeField.interactable = false;

        _timeField.onValueChanged.AddListener(ApplyTimeMask);
    }

    private void OnEnable()
    {
        _timeManager.Time—hanged += UpdateView;
        _controls.ChangeModeEnabled += ChangeTimeManually;
    }

    private void OnDisable()
    {
        _timeManager.Time—hanged -= UpdateView;
        _controls.ChangeModeEnabled -= ChangeTimeManually;
        _timeField.onValueChanged.RemoveAllListeners();
    }

    private void Update()
    {
        if (_changeModeEnabled)
        {
            if(!_timeField.isFocused)
            {
                _currentTime = GetTimeFromArrows();
            }
            else
            {
                _currentTime = GetTimeFromInputField();
            }
            
            _previousHour = _currentTime.Hour;
            
            TimeManually—hanged?.Invoke(_currentTime);
        }
    }

    private void UpdateView(DateTime currentTime)
    {
        _currentTime = currentTime;

        _timeField.text = _currentTime.ToString("HH:mm:ss");

        _hoursArrow.DOLocalRotate(new Vector3(0f, 0f, _currentTime.Hour * -_hoursToDegrees), 1f);
        _minutesArrow.DOLocalRotate(new Vector3(0f, 0f, _currentTime.Minute * -_minutesToDegrees), 1f);
        _secondsArrow.DOLocalRotate(new Vector3(0f, 0f, _currentTime.Second * -_secondsToDegrees), 1f);
    }

    private void ChangeTimeManually(bool changeModeEnabled)
    {
        _changeModeEnabled = changeModeEnabled;

        if (_changeModeEnabled)
        {
            DragArrow[] dragArrows = FindObjectsOfType<DragArrow>();
            foreach (var dragArrow in dragArrows)
            {
                dragArrow.gameObject.GetComponent<DragArrow>().enabled = true;
            }

            _timeField.interactable = true;
        }
        else
        {
            DragArrow[] dragArrows = FindObjectsOfType<DragArrow>();
            foreach (var dragArrow in dragArrows)
            {
                dragArrow.gameObject.GetComponent<DragArrow>().enabled = false;
            }

            _timeField.interactable = false;
        }
    }

    private DateTime GetTimeFromArrows()
    {
        return new DateTime(_currentTime.Year, _currentTime.Month, _currentTime.Day,
                                        GetHourFromArrow(), GetMinuteFromArrow(), GetSecondFromArrow());
    }

    private int GetHourFromArrow()
    {
        float zRotation = _hoursArrow.transform.localEulerAngles.z;

        zRotation -= 360;

        float currentHour = -zRotation / _hoursToDegrees;

        if (_previousHour >= 12)
        {
            currentHour += 12;
        }

        if ((int)Mathf.Round(currentHour) == 24)
        {
            return 0;
        }
        else
        {
            return (int)Mathf.Round(currentHour);
        }
    }

    private int GetMinuteFromArrow()
    {
        float zRotation = _minutesArrow.transform.localEulerAngles.z;

        zRotation -= 360;

        float minute = -zRotation / _minutesToDegrees;

        return (int)Mathf.Round(minute);
    }

    private int GetSecondFromArrow()
    {
        float zRotation = _secondsArrow.transform.localEulerAngles.z;

        zRotation -= 360;

        float second = -zRotation / _secondsToDegrees;

        return (int)Mathf.Round(second);
    }

    private void ApplyTimeMask(string inputText)
    {
        string pattern = @"^([01]\d|2[0-3]):([0-5]\d):([0-5]\d)$";

        if (inputText.Length > 8)
        {
            inputText = inputText.Substring(0, 8);
        }

        if (!Regex.IsMatch(inputText, pattern))
        {
            inputText = Regex.Replace(inputText, @"^(\d{2})(\d{2})(\d{2})", "$1:$2:$3");

            string[] nums = _timeField.text.Split(":");

            if (nums.Length < 3)
            {
                inputText += "0";
            }
        }

        

        _timeField.text = inputText;
    }

    private DateTime GetTimeFromInputField()
    {
        string[] nums = _timeField.text.Split(":");

        int hours = int.Parse(nums[0]);
        if (hours > 23)
            hours = 23;

        int minutes = int.Parse(nums[1]);
        if (minutes > 59)
            minutes = 59;
        
        int seconds = int.Parse(nums[2]);
        if (seconds > 59)
            seconds = 59;

        return new DateTime(_currentTime.Year, _currentTime.Month, _currentTime.Day,
                                        hours, minutes, seconds);
    }
}