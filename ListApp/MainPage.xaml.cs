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
        const string NEW_ITEM_PLACEHOLDER_TEXT = "Enter new item here.";
        const string ADDING_MODE = "In ADDING mode.";
        const string DELETING_MODE = "In DELETING mode.";
        string CurrMode = ADDING_MODE;
        bool isAddingCategory = false;

        ListView SelectedList;
        public MainPage()
        {
            this.InitializeComponent();
            SelectedList = MainList;
            CurrModeBlk.Text = CurrMode;
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
                if (isAddingCategory)
                {
                    //adding new category, i.e. a new listview
                    ListView NewCat = new ListView();
                    NewCat.SelectionChanged += List_OnSelectionChanged;
                    NewCat.Margin = new Windows.UI.Xaml.Thickness(30, 0, 0, 0);
                    NewCat.MinWidth = 1000;

                    TextBlock NewCatName = new TextBlock();
                    NewCatName.Tapped += CatToLV_Tapped;
                    NewCatName.Text = NewItemTB.Text;
                    NewCatName.HorizontalAlignment = HorizontalAlignment.Stretch;
                    NewCatName.MinWidth = 350;
                   
                    NewCat.Items.Insert(0, NewCatName);
                    SelectedList.Items.Insert(SelectedList.Items.Count - 1, NewCat);
                }
                else
                {
                    //adding new item with a checkbox
                    CheckBox NewItem = new CheckBox();
                    NewItem.Content = NewItemTB.Text;
                    NewItem.Margin = new Windows.UI.Xaml.Thickness(30, 0, 0, 0);
                    SelectedList.Items.Insert(SelectedList.Items.Count - 1, NewItem);
                }

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

        private void AddingBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrMode = ADDING_MODE;
            CurrModeBlk.Text = CurrMode;
        }

        private void DeletingBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrMode = DELETING_MODE;
            CurrModeBlk.Text = CurrMode;
        }

        private void CatToLV_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //tapping category will look like tapping listview

            //get listview containing the tapped category
            TextBlock Cat = (TextBlock)sender;
            Windows.UI.Xaml.DependencyObject CatList = Cat;
            do
            {
                CatList = VisualTreeHelper.GetParent(CatList);
            }
            while (!(CatList is ListView));

            //move new item input field to "tapped" listview
            MoveNewItemField((ListView)CatList);
        }

        private void List_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //user selected an item, presumably to add new items under.

            var SelectedItem = e.AddedItems?.FirstOrDefault();

            //CurrModeBlk.Text = SelectedItem.ToString()+"; "+(SelectedItem is ListView).ToString();

            if (SelectedItem is ListView)
            {
                //item is a category (characterized by being a list view) & is valid to add items under.
                
                //insert new item field at end of selected list
                MoveNewItemField((ListView)SelectedItem);

            }
        }

        private void MoveNewItemField(ListView destList)
        {
            StackPanel temp = NewItemField;
            SelectedList.Items.RemoveAt(SelectedList.Items.Count - 1);
            SelectedList = destList;
            SelectedList.Items.Insert(SelectedList.Items.Count, temp);
            string tempName = temp.Name;
        }

        private void CategoryOption_Checked(object sender, RoutedEventArgs e)
        {
            isAddingCategory = true;

            //hide checkbox for new item field
            NewItemCB.Visibility = Visibility.Collapsed;

        }

        private void CategoryOption_Unchecked(object sender, RoutedEventArgs e)
        {
            isAddingCategory = false;

            //show checkbox for new item field
            NewItemCB.Visibility = Visibility.Visible;
        }


    }
}
