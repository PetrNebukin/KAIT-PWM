using PracticalWorksManager.Models;
using PracticalWorksManager.Services;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PracticalWorksManager.Views
{
    public sealed partial class GroupsPage : Page
    {
        private DatabaseService _databaseService;
        private ObservableCollection<Group> _groups;
        private Group _selectedGroup;

        public GroupsPage()
        {
            this.InitializeComponent();
            _databaseService = new DatabaseService();
            _groups = new ObservableCollection<Group>(); // Initialize here
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadGroups();
        }

        private async void LoadGroups()
        {
            _groups = _databaseService.GetGroups();
            GroupsListView.ItemsSource = _groups;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(GroupNameTextBox.Text))
            {
                Group newGroup = new Group { GroupName = GroupNameTextBox.Text };
                _databaseService.AddGroup(newGroup);
                newGroup.GroupID = _databaseService.GetLastGroupId(); // Get autoincremented ID
                _groups.Add(newGroup);
                GroupNameTextBox.Text = string.Empty;
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGroup != null && !string.IsNullOrEmpty(GroupNameTextBox.Text))
            {
                _selectedGroup.GroupName = GroupNameTextBox.Text;
                _databaseService.UpdateGroup(_selectedGroup);
                //No need to update _groups, because element already binded
                GroupNameTextBox.Text = string.Empty;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGroup != null)
            {
                _databaseService.DeleteGroup(_selectedGroup.GroupID);
                _groups.Remove(_selectedGroup);
                GroupNameTextBox.Text = string.Empty;
                _selectedGroup = null;
            }
        }

        private void GroupsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            _selectedGroup = (Group)e.ClickedItem;
            GroupNameTextBox.Text = _selectedGroup.GroupName;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}