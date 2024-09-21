using System;
using System.Collections;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ServerTimeProvider : MonoBehaviour
{
    [SerializeField] private Controls _controls;

    private static HttpClient httpClient = new HttpClient();

    private string _url = "https://yandex.com/time/sync.json";
    private bool _isRunning = true;

    public event Action<DateTime> TimeIsLoaded;

    private void Start()
    {
        // Вариант с асинхронностью, System.Net.Http и using System.Threading.Tasks
        GetTimeAsync();

        // Вариант с корутиной и using UnityEngine.Networking
        //StartCoroutine(GetTimeRoutine());
    }

    private void OnEnable()
    {
        _controls.ChangeModeEnabled += StopServerTimeProvider;
        _controls.ManuallyModeDisabled += ResumeServerTimeProvider;
    }

    private void OnDisable()
    {
        _controls.ChangeModeEnabled -= StopServerTimeProvider;
        _controls.ManuallyModeDisabled -= ResumeServerTimeProvider;
    }

    private async void GetTimeAsync()
    {
        await GetRequest();

        RepeatGetRequestWithDelay();
    }

    private async void RepeatGetRequestWithDelay()
    {
        if (_isRunning)
        {
            var task = Task.Run(() =>
            {
                Thread.Sleep(3600 * 1000);
            });

            await GetRequest();

            await task;

            if (Application.isPlaying)
            {
                RepeatGetRequestWithDelay();
            }
        }
    }

    private async Task GetRequest()
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _url);

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            YandexTimeData timeResponse = JsonUtility.FromJson<YandexTimeData>(content);

            DateTime currentTime = DateTimeOffset.FromUnixTimeMilliseconds(timeResponse.time).LocalDateTime;

            TimeIsLoaded?.Invoke(currentTime);
        }
        else
        {
            Debug.Log($"Ошибка при запросе времени: response status {response.StatusCode}");
        }
    }

    private IEnumerator GetTimeRoutine()
    {
        UnityWebRequest request = UnityWebRequest.Get(_url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;

            YandexTimeData timeResponse = JsonUtility.FromJson<YandexTimeData>(jsonResponse);

            DateTime currentTime = DateTimeOffset.FromUnixTimeMilliseconds(timeResponse.time).LocalDateTime;

            TimeIsLoaded?.Invoke(currentTime);
        }
        else
        {
            Debug.LogError("Ошибка при запросе времени: " + request.error);
        }

        yield return new WaitForSecondsRealtime(3600f);

        StartCoroutine(GetTimeRoutine());
    }

    private void StopServerTimeProvider(bool changeModeEnabled)
    {
        if (changeModeEnabled)
        {
            _isRunning = false;
        }
    }

    private void ResumeServerTimeProvider()
    {
        _isRunning = true;

        GetTimeAsync();
    }
}
