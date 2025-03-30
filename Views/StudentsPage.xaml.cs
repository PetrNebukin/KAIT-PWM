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
    public sealed partial class StudentsPage : Page
    {
        private DatabaseService _databaseService;
        private ObservableCollection<Student> _students;
        private ObservableCollection<Group> _groups;
        private Student _selectedStudent;
        private int _selectedGroupId;

        public StudentsPage()
        {
            this.InitializeComponent();
            _databaseService = new DatabaseService();
            _students = new ObservableCollection<Student>();
            _groups = new ObservableCollection<Group>();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadGroups();
            //LoadStudents(); // Load students after groups are loaded
        }

        private async void LoadGroups()
        {
            _groups = _databaseService.GetGroups();
            GroupComboBox.ItemsSource = _groups;
        }

        private async void LoadStudents()
        {
            _students.Clear();
            if (_selectedGroupId != 0)
            {
                var students = _databaseService.GetStudents(_selectedGroupId);
                foreach (var student in students)
                {
                    _students.Add(student);
                }
            }
            StudentsListView.ItemsSource = _students;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGroupId != 0 && !string.IsNullOrEmpty(FirstNameTextBox.Text) && !string.IsNullOrEmpty(LastNameTextBox.Text))
            {
                Student newStudent = new Student
                {
                    GroupID = _selectedGroupId,
                    FirstName = FirstNameTextBox.Text,
                    LastName = LastNameTextBox.Text
                };
                _databaseService.AddStudent(newStudent);
                newStudent.StudentID = _databaseService.GetLastStudentId(); // Get autoincremented ID
                _students.Add(newStudent);
                FirstNameTextBox.Text = string.Empty;
                LastNameTextBox.Text = string.Empty;
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStudent != null && _selectedGroupId != 0 && !string.IsNullOrEmpty(FirstNameTextBox.Text) && !string.IsNullOrEmpty(LastNameTextBox.Text))
            {
                _selectedStudent.GroupID = _selectedGroupId;
                _selectedStudent.FirstName = FirstNameTextBox.Text;
                _selectedStudent.LastName = LastNameTextBox.Text;
                _databaseService.UpdateStudent(_selectedStudent);

                // Refresh the list
                LoadStudents();
                FirstNameTextBox.Text = string.Empty;
                LastNameTextBox.Text = string.Empty;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStudent != null)
            {
                _databaseService.DeleteStudent(_selectedStudent.StudentID);
                _students.Remove(_selectedStudent);
                FirstNameTextBox.Text = string.Empty;
                LastNameTextBox.Text = string.Empty;
                _selectedStudent = null;
            }
        }

        private void StudentsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            _selectedStudent = (Student)e.ClickedItem;
            if (_selectedStudent != null)
            {
                FirstNameTextBox.Text = _selectedStudent.FirstName;
                LastNameTextBox.Text = _selectedStudent.LastName;
                GroupComboBox.SelectedValue = _selectedStudent.GroupID;
            }
        }

        private void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupComboBox.SelectedItem != null)
            {
                _selectedGroupId = ((Group)GroupComboBox.SelectedItem).GroupID;
                LoadStudents();
            }
        }
    }
}