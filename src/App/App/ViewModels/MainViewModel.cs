using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _imageUrl = "https://i.pinimg.com/736x/6a/05/12/6a0512fe6231884a99be9a7c31df2e2e.jpg";

    [RelayCommand]
    private void OpenHatch() //TODO: Run this in a seperate thread
    {
        System.Console.WriteLine("Opened Hatch");
    }
}
