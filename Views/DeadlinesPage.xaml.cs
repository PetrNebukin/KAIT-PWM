using PracticalWorksManager.Models;
using PracticalWorksManager.Services;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Linq;

namespace PracticalWorksManager.Views
{
    public sealed partial class DeadlinesPage : Page
    {
        private DatabaseService _databaseService;
        private ObservableCollection<Deadline> _deadlines;
        private ObservableCollection<Student> _students;
        private ObservableCollection<Assignment> _assignments;
        private Deadline _selectedDeadline;
        private int _selectedStudentId;
        private int _selectedGroupId;
        public DeadlinesPage()
        {
            this.InitializeComponent();
            _databaseService = new DatabaseService();
            _deadlines = new ObservableCollection<Deadline>();
            _students = new ObservableCollection<Student>();
            _assignments = new ObservableCollection<Assignment>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadAssignments();
            LoadGroups();
        }
        private async void LoadGroups()
        {
            GroupsCombo.ItemsSource = _databaseService.GetGroups();
        }

        private async void LoadStudents()
        {
            if (_selectedGroupId == 0) return;
            _students = _databaseService.GetStudents(_selectedGroupId);
            StudentComboBox.ItemsSource = _students;
        }

        private async void LoadAssignments()
        {
            _assignments = _databaseService.GetAssignments();
            AssignmentComboBox.ItemsSource = _assignments;
        }

        private async void LoadDeadlines()
        {
            _deadlines.Clear();
            if (_selectedStudentId != 0)
            {
                var deadlines = _databaseService.GetDeadlines(_selectedStudentId);
                foreach (var deadline in deadlines)
                {
                    _deadlines.Add(deadline);
                }
            }
            DeadlinesListView.ItemsSource = _deadlines;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStudentId != 0 && AssignmentComboBox.SelectedValue != null)
            {
                Deadline newDeadline = new Deadline
                {
                    StudentID = _selectedStudentId,
                    AssignmentID = (int)AssignmentComboBox.SelectedValue,
                    DeadlineDate = DeadlineDatePicker.Date.DateTime,
                    SubmissionDate = SubmissionDatePicker.Date.DateTime
                };
                _databaseService.AddDeadline(newDeadline);
                LoadDeadlines();
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDeadline != null && AssignmentComboBox.SelectedValue != null)
            {
                _selectedDeadline.StudentID = _selectedStudentId;
                _selectedDeadline.AssignmentID = (int)AssignmentComboBox.SelectedValue;
                _selectedDeadline.DeadlineDate = DeadlineDatePicker.Date.DateTime;
                _selectedDeadline.SubmissionDate = SubmissionDatePicker.Date.DateTime;
                _databaseService.UpdateDeadline(_selectedDeadline);
                LoadDeadlines();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDeadline != null)
            {
                _databaseService.DeleteDeadline(_selectedDeadline.DeadlineID);
                _deadlines.Remove(_selectedDeadline);
                LoadDeadlines();
                _selectedDeadline = null;
            }
        }

        private void DeadlinesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            _selectedDeadline = (Deadline)e.ClickedItem;
            if (_selectedDeadline != null)
            {
                AssignmentComboBox.SelectedValue = _selectedDeadline.AssignmentID;
                DeadlineDatePicker.Date = _selectedDeadline.DeadlineDate;
                SubmissionDatePicker.Date = SubmissionDatePicker.Date.DateTime;
                if (_selectedDeadline.SubmissionDate != null)
                {
                    SubmissionDatePicker.Date = _selectedDeadline.SubmissionDate.Value;
                }
            }
        }

        private void StudentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentComboBox.SelectedItem != null)
            {
                _selectedStudentId = ((Student)StudentComboBox.SelectedItem).StudentID;
                LoadDeadlines();
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void GroupsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupsCombo.SelectedItem == null) return;

            _selectedGroupId = ((Group)GroupsCombo.SelectedItem).GroupID;
            LoadStudents();
        }
    }
}