using PracticalWorksManager.Models;
using PracticalWorksManager.Services;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PracticalWorksManager.Views
{
    public sealed partial class AssignmentsPage : Page
    {
        private DatabaseService _databaseService;
        private ObservableCollection<Assignment> _assignments;
        private Assignment _selectedAssignment;

        public AssignmentsPage()
        {
            this.InitializeComponent();
            _databaseService = new DatabaseService();
            _assignments = new ObservableCollection<Assignment>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadAssignments();
        }

        private async void LoadAssignments()
        {
            _assignments = _databaseService.GetAssignments();
            AssignmentsListView.ItemsSource = _assignments;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(AssignmentNameTextBox.Text))
            {
                Assignment newAssignment = new Assignment
                {
                    AssignmentName = AssignmentNameTextBox.Text,
                    Description = DescriptionTextBox.Text
                };
                _databaseService.AddAssignment(newAssignment);
                newAssignment.AssignmentID = _databaseService.GetLastAssignmentId(); // Get autoincremented ID
                _assignments.Add(newAssignment);
                AssignmentNameTextBox.Text = string.Empty;
                DescriptionTextBox.Text = string.Empty;
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAssignment != null && !string.IsNullOrEmpty(AssignmentNameTextBox.Text))
            {
                _selectedAssignment.AssignmentName = AssignmentNameTextBox.Text;
                _selectedAssignment.Description = DescriptionTextBox.Text;
                _databaseService.UpdateAssignment(_selectedAssignment);
                LoadAssignments();
                AssignmentNameTextBox.Text = string.Empty;
                DescriptionTextBox.Text = string.Empty;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAssignment != null)
            {
                _databaseService.DeleteAssignment(_selectedAssignment.AssignmentID);
                _assignments.Remove(_selectedAssignment);
                AssignmentNameTextBox.Text = string.Empty;
                DescriptionTextBox.Text = string.Empty;
                _selectedAssignment = null;
            }
        }

        private void AssignmentsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            _selectedAssignment = (Assignment)e.ClickedItem;
            if (_selectedAssignment != null)
            {
                AssignmentNameTextBox.Text = _selectedAssignment.AssignmentName;
                DescriptionTextBox.Text = _selectedAssignment.Description;
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}