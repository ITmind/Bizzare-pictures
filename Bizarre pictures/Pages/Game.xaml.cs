using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Bizarre_pictures
{
    public partial class Game
    {
        readonly DispatcherTimer _myDispatcherTimer = new DispatcherTimer();
        GameGrid _gameGrid;
        TimeSpan _currentTime = new TimeSpan(0, 0, 0);
        readonly TimeSpan seconds = new TimeSpan(0, 0, 1);

        public SettingsData Settings
        {
            get { return App.Current.Settings; }
        }

        public Game()
        {
            DataContext = this;
            InitializeComponent();
            App.Current.Settings.FieldHeight = 11;
            App.Current.Settings.FieldWidth = 18;
            App.Current.Settings.Player1Score = 0;
            App.Current.Settings.CurGameN=0;
           
            CreateGameGrid();
        }

        void gameGrid_NextLevel(object sender, EventArgs e)
        {
            _myDispatcherTimer.Stop();
            App.Current.Settings.Player1Score += (int)_currentTime.TotalSeconds * 100;
            App.Current.Settings.CurGameN++;
            _gameGrid = null;
            App.Current.Settings.GameTime = new TimeSpan(0, App.Current.Settings.CurGameN < 8 ? 10 - App.Current.Settings.CurGameN : 3, 0);
            CreateGameGrid();
        }

        void CreateGameGrid()
        {
            var numBackground = App.Current.RandomGenerator.Next(1, 7);
            ((ImageBrush)LayoutRoot.Background).ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Background"+numBackground+".jpg")); ;

            _currentTime = new TimeSpan(0, 0, 0);
            timeProgressBar.Maximum = App.Current.Settings.GameTime.TotalSeconds;
            timeProgressBar.Value = timeProgressBar.Maximum;

            ContentPanel.Children.Clear();
            App.Current.Settings.Grid.Clear();
            tbNumSearch.Text = 3.ToString();            

            _gameGrid = new GameGrid();
            _gameGrid.NextLevel += gameGrid_NextLevel;
            _gameGrid.EndGame += gameGrid_EndGame;
            _gameGrid.ManipulationDelta += gameGrid_ManipulationDelta;

            ContentPanel.Children.Add(_gameGrid);

            //tbTime.Text = String.Format("{0:D2}:{1:D2}", App.Current.Settings.GameTime.Minutes, App.Current.Settings.GameTime.Seconds);
            _myDispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _myDispatcherTimer.Tick += Each_Tick;
            _myDispatcherTimer.Start();
        }


        void gameGrid_EndGame(object sender, EventArgs e)
        {
            _myDispatcherTimer.Stop();
            App.Current.Settings.Player1Score += (int)_currentTime.TotalSeconds * 100;
            EndGame();
        }

        void gameGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            
        }

        public void Each_Tick(object o, object sender)
        {
            _currentTime = _currentTime.Add(seconds);
            //TimeSpan temp = App.Current.Settings.GameTime.Subtract(_currentTime);
            //tbTime.Text = String.Format("{0:D2}:{1:D2}", temp.Minutes, temp.Seconds);
            timeProgressBar.Value--;
            if (_currentTime >= App.Current.Settings.GameTime)
            {
                _myDispatcherTimer.Stop();
                Frame.Navigate(typeof(GameOver));
            }
        }

        private void timeProgressBar_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if(e.NewValue == 60) ((ProgressBar)sender).Foreground = new SolidColorBrush(Colors.Red);
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            HelpPopup.IsOpen = true;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ShowSolver();
        }

        void ShowSolver()
        {
            var curTag = Convert.ToInt32(tbNumSearch.Text);
            if (curTag > 0)
            {
                _gameGrid.ShowHelp();
                curTag--;
                tbNumSearch.Text = curTag.ToString();
            }
        }

        void ShowAntibos()
        {
            
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Window.Current.CoreWindow.KeyDown += delegate(CoreWindow sender, KeyEventArgs args)
                                                     {
                                                         if (args.VirtualKey == VirtualKey.Escape)
                                                         {
                                                             EndGame();
                                                         }
                                                         else if (args.VirtualKey == VirtualKey.H)
                                                         {
                                                             ShowSolver();
                                                         }
                                                         else if (args.VirtualKey == VirtualKey.Q)
                                                         {
                                                             //antiboss
                                                             App.Current.Exit();
                                                         }
                                                     };
        }

        private void LayoutRoot_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(LeaderboardPopup.IsOpen)
            {
                //new game
                LeaderboardPopup.IsOpen = false;
                App.Current.Settings.Player1Score = 0;
                App.Current.Settings.GameTime = new TimeSpan(0, 10, 0);
                CreateGameGrid();
            }

            if (HelpPopup.IsOpen && (string)HelpPopup.Tag == "open")
            {
                HelpPopup.IsOpen = false;
            }
        }

        private void EndGame()
        {
            _myDispatcherTimer.Stop();
            var score = new Score
            {
                Points = App.Current.Settings.Player1Score,
                UserName = App.Current.Settings.Player1Name,
                Data = 0
            };
            App.Current.Settings.Scores.Add(score);

            ucLeaderboard.Show();

            LeaderboardPopup.IsOpen = true;
        }

        private void HelpPopup_Opened(object sender, object e)
        {
            HelpPopup.Tag = "open";
        }

        private void HelpPopup_Closed(object sender, object e)
        {
            HelpPopup.Tag = "";
        }
    }
}