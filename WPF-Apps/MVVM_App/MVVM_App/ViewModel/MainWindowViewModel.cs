using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
