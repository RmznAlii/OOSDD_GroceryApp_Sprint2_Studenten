using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.App.Views;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    [QueryProperty(nameof(GroceryList), nameof(GroceryList))]
    public partial class GroceryListItemsViewModel : BaseViewModel
    {
        private readonly IGroceryListItemsService _groceryListItemsService;
        private readonly IProductService _productService;
        public ObservableCollection<GroceryListItem> MyGroceryListItems { get; set; } = [];
        public ObservableCollection<Product> AvailableProducts { get; set; } = [];

        [ObservableProperty]
        GroceryList groceryList = new(0, "None", DateOnly.MinValue, "", 0);

        public GroceryListItemsViewModel(IGroceryListItemsService groceryListItemsService, IProductService productService)
        {
            _groceryListItemsService = groceryListItemsService;
            _productService = productService;
            Load(groceryList.Id);
        }

        private void Load(int id)
        {
            MyGroceryListItems.Clear();
            foreach (var item in _groceryListItemsService.GetAllOnGroceryListId(id)) MyGroceryListItems.Add(item);
            GetAvailableProducts();
        }

        private void GetAvailableProducts()
        {
            AvailableProducts.Clear();
            var products = _productService.GetAll();
            foreach (var product in products)
            {
                bool existingProduct = MyGroceryListItems.Any(item => item.ProductId == product.Id);
                if (!existingProduct && product.Stock > 0)
                {
                    AvailableProducts.Add(product);
                }
            }
            
            //Maak de lijst AvailableProducts leeg | gedaan
            //Haal de lijst met producten op / gedaan 
            //Controleer of het product al op de boodschappenlijst staat, zo niet zet het in de AvailableProducts lijst / gedaan
            //Houdt rekening met de voorraad (als die nul is kun je het niet meer aanbieden). / gedaan         
        }

        partial void OnGroceryListChanged(GroceryList value)
        {
            Load(value.Id);
        }

        [RelayCommand]
        public async Task ChangeColor()
        {
            Dictionary<string, object> paramater = new() { { nameof(GroceryList), GroceryList } };
            await Shell.Current.GoToAsync($"{nameof(ChangeColorView)}?Name={GroceryList.Name}", true, paramater);
        }
        [RelayCommand]
        public void AddProduct(Product product)
        {
            if (product != null && product.Id > 0)

            {
                var groceryList = new GroceryListItem(0, GroceryList.Id, product.Id, 1);;
                _groceryListItemsService.Add(groceryList);
                product.Stock--;
                _productService.Update(product);
                if (product.Stock <= 0) AvailableProducts.Remove(product);
                OnGroceryListChanged(GroceryList);
            }
            //Controleer of het product bestaat en dat de Id > 0 / gedaan
            //Maak een GroceryListItem met Id 0 en vul de juiste productid en grocerylistid / gedaan
            //Voeg het GroceryListItem toe aan de dataset middels de _groceryListItemsService / gedaan 
            //Werk de voorraad (Stock) van het product bij en zorg dat deze wordt vastgelegd (middels _productService) / gedaan
            //Werk de lijst AvailableProducts bij, want dit product is niet meer beschikbaar / gedaan
            //call OnGroceryListChanged(GroceryList); / gedaan
        }
    }
}
