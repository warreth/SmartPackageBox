using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace App.ViewModels;

public partial class MainViewModel : ViewModelBase
{

    [ObservableProperty]
    private string _imageUrl = "https://picsum.photos/200/300";

    [ObservableProperty]
    private string _imageTimestamp = "";

    public string FullImageUrl => $"{ImageUrl}?t={ImageTimestamp}";

    [RelayCommand]
    private void RefreshImage()
    {
        ImageTimestamp = DateTime.Now.Ticks.ToString();
        OnPropertyChanged(nameof(FullImageUrl)); //Or OnPropertyChanged("FullImageUrl"); -> Let the UI know that the property has changed

        //System.Console.WriteLine("Refreshed Image");
        //System.Console.WriteLine($"Url is {ImageUrl}?t={ImageTimestamp}");
    }

    [RelayCommand]
    private void OpenHatch() //TODO: Run this in a seperate thread or make async so the UI can't freeze
    {
        System.Console.WriteLine("Opened Hatch");
    }
}
