using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class GroceryListViewModel : BaseViewModel
    {
        public ObservableCollection<GroceryList> GroceryLists { get; set; }
        private readonly IGroceryListService _groceryListService;
        private readonly IAuthService _authService;
        private readonly GlobalViewModel _global;
        
        [ObservableProperty]
        private string loggedInUserName;
        public GroceryListViewModel(IGroceryListService groceryListService, GlobalViewModel global) 
        {
            Title = "Boodschappenlijst";
            _groceryListService = groceryListService;
            _global = global;
            GroceryLists = new ObservableCollection<GroceryList>();
        }

        [RelayCommand]
        public async Task SelectGroceryList(GroceryList groceryList)
        {
            Dictionary<string, object> paramater = new() { { nameof(GroceryList), groceryList } };
            await Shell.Current.GoToAsync($"{nameof(Views.GroceryListItemsView)}?Titel={groceryList.Name}", true, paramater);
        }
        public override void OnAppearing()
        {
            base.OnAppearing();
            var loggedInUser = _global.Client;
            if (loggedInUser != null)
            {
                LoggedInUserName = loggedInUser.Name;
                GroceryLists.Clear();
                var userLists = _groceryListService.GetAll()
                    .Where(list => list.ClientId == loggedInUser.Id);
                foreach (var list in userLists)
                {
                    GroceryLists.Add(list);
                }
            }
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            GroceryLists.Clear();
        }
    }
}
