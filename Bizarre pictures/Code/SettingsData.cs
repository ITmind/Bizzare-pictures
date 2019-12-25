using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;

namespace Bizarre_pictures
{
    public class SettingsData : INotifyPropertyChanged
    {

        int _player1Score;
        ObservableCollection<int> _grid;

        public ObservableCollection<int> Grid
        {
            get
            {
                return _grid;
            }
            set
            {
                _grid = value;
                NotifyPropertyChanged("Grid");
            }
        }

        public int Player1Score
        {
            get
            {
                return _player1Score;
            }
            set
            {
                _player1Score = value;
                NotifyPropertyChanged("Player1Score");
            }
        }
        public string Player1Name { get; set; }
        public int FieldWidth { get; set; }
        public int FieldHeight { get; set; }
        public int CurGameN { get; set; }
        public TimeSpan GameTime { get; set; }
        public SortedSet<Score> Scores { get; set; }

        

        public SettingsData()
        {
            Grid = new ObservableCollection<int>();
            Scores = new SortedSet<Score>(new InvertedComparer());
            Player1Score = 0;
            Player1Name = "Player1";
            FieldWidth = 10;
            FieldHeight = 5;
            CurGameN = 0;
            GameTime = new TimeSpan(0, 10, 0);
            _grid.CollectionChanged += grid_CollectionChanged;
        }

        void grid_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Grid");
        }

        public void Clear()
        {
            Grid = new ObservableCollection<int>();
            Scores = new SortedSet<Score>(new InvertedComparer());
            Player1Score = 0;
            Player1Name = "Player1";
            FieldWidth = 10;
            FieldHeight = 5;
            CurGameN = 0;
            GameTime = new TimeSpan(0, 10, 0);
            _grid.CollectionChanged += grid_CollectionChanged;
        }

        public void ShrinkScore()
        {
            int i = 0;
            foreach (var score in Scores)
            {
                i++;
                score.Data = i;
            }

            Scores.RemoveWhere(x => x.Data > 5);
        }

        public static async Task<SettingsData> Load()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync("settings.dat");
                var serializedString = await FileIO.ReadTextAsync(file);
                return await JsonConvert.DeserializeObjectAsync<SettingsData>(serializedString);
            }
            catch (Exception)
            {

                return new SettingsData();
            }

        }

        public static async void Save(SettingsData settings)
        {
            var serializedString = JsonConvert.SerializeObject(settings);
            var storageFolder = ApplicationData.Current.LocalFolder;
            var file = await storageFolder.CreateFileAsync("settings.dat", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, serializedString);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion   
    }

    public class Score
    {
        public string UserName { get; set; }
        public int Points { get; set; }
        public int Data { get; set; }
    }

    class InvertedComparer : IComparer<Score>
    {
        public int Compare(Score x, Score y)
        {
            int result = 0;
            if(x.Points>y.Points)
            {
                result = -1;
            }
            else if (x.Points < y.Points)
            {
                result = 1;
            }
            return result;
        }
    }
}
