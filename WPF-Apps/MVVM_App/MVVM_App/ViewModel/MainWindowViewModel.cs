using System.Collections.ObjectModel;
using MVVM_App.Model;
using MVVM_App.MVVM;

namespace MVVM_App.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        //We are replacing the main code behind with this view model, we'll stick to data bindings
        //Called this view model from main model

        public ObservableCollection<Item> Items { get; set; }
        public MainWindowViewModel() 
        {
            Items = new ObservableCollection<Item>();
            Items.Add(new Item
            {
                Name = "Iphone",
                SerialNumber = "011",
                Quantity = 1
            });
            Items.Add(new Item
            {
                Name = "Iphone2",
                SerialNumber = "012",
                Quantity = 2
            });
        }

        private Item selectedItem;
        public Item SelectedItem
        {
            get { return selectedItem; }
            set 
            { 
                SelectedItem = value;
                onPropertyChanged();
            }
            
        }
        

    }
}
