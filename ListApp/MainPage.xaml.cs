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
            InsertTestData();
        }

        void InsertTestData()
        {
            for (int i = 0; i < 12; i++)
            {
                if (i % 4 == 0)
                {
                    AddNewCat(i.ToString(), SelectedList);
                }
                else
                {
                    AddNewItem(i.ToString(), SelectedList);
                }
            }
        }

        void NewItemTB_GotFocus(object sender, RoutedEventArgs e)
        {
            NewItemTB.PlaceholderText = "";
        }

        public ScrollViewer GetScrollViewer(DependencyObject element)
        {
            if (element is ScrollViewer)
            {
                return (ScrollViewer)element;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }

            return null;
        }


        void AddNewCat(string catNameString, ListView destList)
        {
            //adding new category, i.e. a new listview
            ListView newCat = new ListView();
            newCat.SelectionChanged += List_OnSelectionChanged;
            newCat.Margin = new Windows.UI.Xaml.Thickness(30, 0, 0, 0);
            newCat.MinWidth = 360;

            TextBlock newCatName = new TextBlock();
            newCatName.Tapped += CatToLV_Tapped;
            newCatName.Text = catNameString;
            newCatName.MinWidth = 360;
            newCatName.IsTextSelectionEnabled = false;

            newCat.Items.Insert(0, newCatName);
            destList.Items.Insert(destList.Items.Count - 1, newCat);

            //ScrollViewer newCatSV = GetScrollViewer((DependencyObject)MainList);
            DependencyObject o = VisualTreeHelper.GetChild(MainList, 0);
            int numVisuals = VisualTreeHelper.GetChildrenCount(MainList);
            //newCatSV.VerticalScrollMode = ScrollMode.Disabled;
        }

        void AddNewItem(string itemNameString, ListView destList)
        {
            //adding new item with a checkbox
            CheckBox newItem = new CheckBox();
            newItem.Content = itemNameString;
            newItem.Margin = new Windows.UI.Xaml.Thickness(30, 0, 0, 0);
            destList.Items.Insert(destList.Items.Count - 1, newItem);
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
                    AddNewCat(NewItemTB.Text, SelectedList);
                }
                else
                {
                    AddNewItem(NewItemTB.Text, SelectedList);
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

            //show elements related to addition
            CategoryOption.Visibility = Windows.UI.Xaml.Visibility.Visible;
            NewItemField.Visibility = Windows.UI.Xaml.Visibility.Visible;
            GetParentLVIOf(NewItemField).Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void DeletingBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrMode = DELETING_MODE;
            CurrModeBlk.Text = CurrMode;

            //hide elements not related to deletion
            CategoryOption.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            NewItemField.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            GetParentLVIOf(NewItemField).Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private ListView GetParentListOf(DependencyObject child)
        {
            if (child == MainList)
                return null;

            DependencyObject ParentLV = child;
            do
            {
                ParentLV = VisualTreeHelper.GetParent(ParentLV);
            }
            while (!(ParentLV is ListView));

            return (ListView)ParentLV;
        }

        private ListViewItem GetParentLVIOf(DependencyObject child)
        {
            if (child == MainList)
                return null;

            DependencyObject ParentLVI = child;
            do
            {
                ParentLVI = VisualTreeHelper.GetParent(ParentLVI);
            }
            while (!(ParentLVI is ListViewItem));

            return (ListViewItem)ParentLVI;
        }

        private void RemoveItem(DependencyObject item)
        {
            if (item == MainList)
                return;

            ListView ContainingList = GetParentListOf(item);

            if (item is CheckBox)
            {
                bool removalStatus = ContainingList.Items.Remove(item);
            }
            else if (item is ListView && item != MainList)
            {
                //ListView FinalContainingList = GetParentListOf(ContainingList);

                bool test = (ContainingList == MainList);
                bool removalStatus = ContainingList.Items.Remove(item);
            }
 
        }

        private void CatToLV_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //tapping category will look like tapping listview

            TextBlock Cat = (TextBlock)sender;


            //get listview containing the tapped category
            Windows.UI.Xaml.DependencyObject CatList = Cat;
            do
            {
                CatList = VisualTreeHelper.GetParent(CatList);
            }
            while (!(CatList is ListView));

            if (CurrMode == ADDING_MODE)
            {
                //move new item input field to "tapped" listview
                MoveNewItemField((ListView)CatList);

                //select the listviewitem corresponding to CatList
                if (CatList != MainList)
                {
                    Windows.UI.Xaml.DependencyObject CatListItem = CatList;
                    do
                    {
                        CatListItem = VisualTreeHelper.GetParent(CatListItem);
                    }
                    while (!(CatListItem is ListViewItem));

                    ((ListViewItem)CatListItem).IsSelected = true;
                }
            }
            else if (CurrMode == DELETING_MODE)
            {
                RemoveItem((DependencyObject)CatList);
            }

        }

        private void List_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //user selected an item
            var SelectedItem = e.AddedItems?.FirstOrDefault();
            if (SelectedItem == null)
                return;

            if (CurrMode == ADDING_MODE)
            {
                //CurrModeBlk.Text = SelectedItem.ToString()+"; "+(SelectedItem is ListView).ToString();

                if (SelectedItem is ListView)
                {
                    //item is a category (characterized by being a list view) & is valid to add items under.

                    //insert new item field at end of selected list
                    MoveNewItemField((ListView)SelectedItem);
                }
            }
            else if (CurrMode == DELETING_MODE)
            {
                if (SelectedItem is CheckBox)
                {
                    RemoveItem((DependencyObject)SelectedItem);
                }
                //textblock (i.e. category) deletion will be handled by CatToLV_Tapped
            }
            else
            { }
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
