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

        public RelayCommand AddCommand => new RelayCommand(execute => AddItem());
        public RelayCommand DeleteCommand => new RelayCommand(execute => DeleteItem(), canExecute => SelectedItem!=null);
        public RelayCommand SaveCommand => new RelayCommand(execute => Save(), canExecute => CanSave());

        public MainWindowViewModel() 
        {
            Items = new ObservableCollection<Item>();
           /* Items.Add(new Item
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
            });*/
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

        private void AddItem()
        {
            Items.Add(new Item
            {
                Name ="Reza",
                SerialNumber="999",
                Quantity = 100
            });
        }

        private void DeleteItem()
        {
            Items.Remove(SelectedItem);
        }

        private void Save()
        {

        }
        private bool CanSave()
        {
            return true;
        }

    }
}
