using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236

namespace Bizarre_pictures.Pages
{
    public sealed partial class Leaderboard : UserControl
    {
        public ObservableCollection<Score> Scores { get; set; }

        public Leaderboard()
        {
            Scores =  new ObservableCollection<Score>();
            this.DataContext = this;
            this.InitializeComponent();
        }

        public void Show()
        {
            Scores.Clear();

            int i = 0;
            foreach (var score in App.Current.Settings.Scores)
            {
                i++;
                score.Data = i;
                Scores.Add(score);
            }

        }
    }
}
