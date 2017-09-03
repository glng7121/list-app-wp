using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ListApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class MainPage : Page
    {
        string NEW_ITEM_PLACEHOLDER_TEXT = "Enter new item here.";
        ListView SelectedList;
        public MainPage()
        {
            this.InitializeComponent();
            SelectedList = MainList;

        }

        void NewItemTB_GotFocus(object sender, RoutedEventArgs e)
        {
            NewItemTB.PlaceholderText = "";
        }

        void NewItemTB_KeyUp(object sender, RoutedEventArgs e)
        {
            if (((KeyRoutedEventArgs)e).Key == Windows.System.VirtualKey.Enter)
            {
                //dismiss keyboard
                NewItemTB.IsEnabled = false;
                NewItemTB.IsEnabled = true;

                //add new item to end of current list
                CheckBox NewItem = new CheckBox();
                NewItem.Content = NewItemTB.Text;
                MainList.Items.Insert(SelectedList.Items.Count - 1, NewItem);
                
                //restore textbox placeholder text
                NewItemTB.Text = "";
                NewItemTB.PlaceholderText = NEW_ITEM_PLACEHOLDER_TEXT;
            }
        }

        void NewItemTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NewItemTB.Text == "")
            {
                NewItemTB.PlaceholderText = NEW_ITEM_PLACEHOLDER_TEXT;
            }
        }
    }
}
