using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Bizarre_pictures
{
    public partial class GameOver : Page
    {        
        private int _score;

        public GameOver()
        {
            DataContext = this;
            InitializeComponent();
            
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            UserName.Text = App.Current.Settings.Player1Name;
            _score = App.Current.Settings.Player1Score;
            textBlock1.Text = String.Format("You score: {0}", _score);
         
            base.OnNavigatedTo(e);
            App.Current.Settings.Grid.Clear();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {

            App.Current.Settings.Player1Name = UserName.Text;
            
            if (string.IsNullOrEmpty(UserName.Text))
            {
                return;
            }
            Submit.IsEnabled = false;
            Submit.Content = "Sending...";

        } 
    }
}