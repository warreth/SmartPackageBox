using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using App.Services;

namespace App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string imageServerUrl = "http://raspberrypi.local:8081/latest.png";

    [ObservableProperty]
    private string apiUrl = "https://raspberrypi.local:8080";


    // Add properties for editing and saving settings
    [ObservableProperty]
    private string? editableApiUrl;

    [ObservableProperty]
    private string? editableImageUrl;

    [ObservableProperty]
    private bool imageVisible = true;

    [ObservableProperty]
    private bool editableImageVisible;

    [ObservableProperty]
    private bool logVisible = true;

    [ObservableProperty]
    private bool editableLogVisible;

    [ObservableProperty]
    private string newestImageUrl = "";

    public ObservableCollection<string> LogMessages { get; } = new ObservableCollection<string>();

    private readonly ApiService _apiService = new ApiService();

    public void Log(string message)
    {
        if (message == null)
        {
            message = "[null log message]";
        }
        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
        LogMessages.Add(logEntry);
    }

    [RelayCommand]
    private async Task RefreshImageAsync()
    {
        string? result = await _apiService.ContactUrlAsync(ApiUrl, "newest-url");
        NewestImageUrl = ImageServerUrl + result;
        Log($"Refreshimage result: {NewestImageUrl}");
        OnPropertyChanged(nameof(LogMessages));
    }

    [RelayCommand]
    private async Task OpenHatchAsync()
    {
        string? result = await _apiService.ContactUrlAsync(ApiUrl, "open-hatch");

        bool isOpen = Convert.ToBoolean(result);
        result = isOpen ? "Hatch is now open" : "Hatch is now closed";
        Log($"Openhatch result: {result}");

        OnPropertyChanged(nameof(LogMessages));
    }

    [RelayCommand]
    private async Task TakePictureAsync()
    {
        string? result = await _apiService.ContactUrlAsync(ApiUrl, "take-picture");

        bool isOpen = Convert.ToBoolean(result);
        result = isOpen ? "Picture taken" : "Picture couldnt be taken (see RPi logs)";
        Log($"Openhatch result: {result}");

        OnPropertyChanged(nameof(LogMessages));
    }

    [RelayCommand]
    private void LoadSettings()
    {
        EditableApiUrl = ApiUrl;
        EditableImageUrl = ImageServerUrl;
        EditableImageVisible = ImageVisible;
        EditableLogVisible = LogVisible;
        Log("Loaded settings into editable fields");
    }

    [RelayCommand]
    private void SaveSettings()
    {
        if (string.IsNullOrWhiteSpace(EditableApiUrl) || string.IsNullOrWhiteSpace(EditableImageUrl))
        {
            Log("API or Image URL cannot be empty");
            return;
        }

        ApiUrl = EditableApiUrl;
        ImageServerUrl = EditableImageUrl;
        ImageVisible = EditableImageVisible;
        LogVisible = EditableLogVisible;
        Log($"Settings saved: API URL = {ApiUrl}, Image URL = {ImageServerUrl}, Image Visible = {ImageVisible}");
    }

    partial void OnApiUrlChanged(string value)
    {
        if (string.IsNullOrEmpty(EditableApiUrl))
            EditableApiUrl = value;
    }

    partial void OnImageServerUrlChanged(string value)
    {
        if (string.IsNullOrEmpty(EditableImageUrl))
            EditableImageUrl = value;
    }

    public MainViewModel()
    {
        EditableApiUrl = ApiUrl;
        EditableImageUrl = ImageServerUrl;
        EditableImageVisible = ImageVisible;
        EditableLogVisible = LogVisible;
    }
}
