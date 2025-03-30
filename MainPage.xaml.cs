using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Security.Principal;


namespace PracticalWorksManager
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void GroupsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Views.GroupsPage));
        }

        private void StudentsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Views.StudentsPage));
        }

        private void AssignmentsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Views.AssignmentsPage));
        }

        private void DeadlinesButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Views.DeadlinesPage));
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            UserName.Text = $"Пользователь: {WindowsIdentity.GetCurrent().Name.Split('\\')[1]}";
            
        }
    }
}