using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Linq;
using Bizarre_pictures.Code;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Bizarre_pictures
{
    public partial class GameGrid
    {

        public event EventHandler EndGame;
        public event EventHandler NextLevel;
        bool _gameEnd;
        

        private readonly List<Border> _selectedBorderList = new List<Border>(10);
        readonly List<BitmapImage> _sprites = new List<BitmapImage>(13);
        readonly List<Path> _pathList = new List<Path>();
        Path _curPath;

        readonly DispatcherTimer _myDispatcherTimer = new DispatcherTimer();

        public int FirstSelect { get; set; }
        public int SecondSelect { get; set; }
        Color _backgroundColor = new HexColor("#FF00303E");
        


        protected void OnEndGame(EventArgs e)
        {
            EventHandler handler = EndGame;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnNextLevel(EventArgs e)
        {
            EventHandler handler = NextLevel;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public GameGrid()
        {
            // Required to initialize variables            
            DataContext = this;
            InitializeComponent();
            Init();
            LoadBitmap();

            GenerateField();

            _myDispatcherTimer.Interval = new TimeSpan(0,0,0,0,200);
            _myDispatcherTimer.Tick += myDispatcherTimer_Tick;
        }

        private void myDispatcherTimer_Tick(object sender, object e)
        { 
            _myDispatcherTimer.Stop();
            App.Current.Settings.Grid[FirstSelect] = -1;
            App.Current.Settings.Grid[SecondSelect] = -1;
            var firstBorder = GetBorder(FirstSelect);
            ((Grid) firstBorder.Child).Children.RemoveAt(0);
            firstBorder.Background = new SolidColorBrush(Colors.Transparent);


            var secondBorder = GetBorder(SecondSelect);
            //SecondBorder.Child = null;
            ((Grid) secondBorder.Child).Children.RemoveAt(0);
            secondBorder.Background = new SolidColorBrush(Colors.Transparent);
            _selectedBorderList.Clear();
            ClearPath();

            switch (App.Current.Settings.CurGameN)
            {
                case 0:
                    
                    break;
                case 1:
                    UpColumn();
                    break;
                case 2:
                    DownColumn();
                    break;
                case 3:
                    LeftRow();
                    break;
                case 4:
                    RightRow();
                    break;
                default:
                    if (App.Current.Settings.CurGameN % 2 > 0)
                    {
                        UpColumn();
                    }
                    else
                    {
                        RightRow();
                    }
                    break;
            }
           
            SetSelectedBordersColor(_backgroundColor);
            _selectedBorderList.Clear();
            FirstSelect = -1;
            SecondSelect = -1;

            bool temp = ChekGrid();
            
            if (IsGridEmpty())
            {
                //end
                OnNextLevel(new EventArgs());
                return;
            }

            if (!temp)
            {
                SortGrid();
                GenerateField();
            }
            FirstSelect = -1;
            SecondSelect = -1;

        }

        void GenerateField()
        {
            LayoutRoot.Children.Clear();
            LayoutRoot.RowDefinitions.Clear();
            LayoutRoot.ColumnDefinitions.Clear();

            //первая пустая маленькая

            for (int row = 0; row < App.Current.Settings.FieldHeight; row++)
            {
                var rd = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
                LayoutRoot.RowDefinitions.Add(rd);
            }
            

            //первая пустая маленькая

            for (int row = 0; row < App.Current.Settings.FieldWidth; row++)
            {
                var cd = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                LayoutRoot.ColumnDefinitions.Add(cd);
            }

            for (int row = 0; row < App.Current.Settings.FieldHeight; row++)
            {
                for (int column = 0; column < App.Current.Settings.FieldWidth; column++)
                {
                    int n = column + App.Current.Settings.FieldWidth * row;
                    var border = new Border {Tag = n};
                    border.Tapped += Border_Tap;
                    //border.BorderBrush = new SolidColorBrush(Colors.Black);
                    var childGrid = new Grid();
                    border.Child = childGrid;
                    if (App.Current.Settings.Grid[n] != -1)
                    {
                        var img = new Image {Source = _sprites[App.Current.Settings.Grid[n]]};

                        childGrid.Children.Add(img);
                    }
                    else
                    {
                        border.Background = new SolidColorBrush(Colors.Transparent);
                    }

                    border.SetValue(Grid.ColumnProperty, column);
                    border.SetValue(Grid.RowProperty, row);

#if DEBUG                    
                    TextBlock tb= new TextBlock();
                    tb.Text = n.ToString();
                    tb.Foreground = new SolidColorBrush(Colors.White);
                    //childGrid.Children.Add(tb);
#endif
                    LayoutRoot.Children.Add(border);

                }
            }
        }

        void LoadBitmap()
        {
            //37 - horiozontal
            //38 - vertical
            //39 - half_down
            //40 - half_left
            //41 - half_right
            //42 - half_up
            //43 - bottomleft
            //44 - bottomright
            //45 - topleft
            //46 - topright

            for (int i = 0; i < 48; i++)
            {
                var uri = new Uri("ms-appx:/Assets/" + i.ToString() + ".png", UriKind.RelativeOrAbsolute);
                var bitmap = new BitmapImage(uri);
                _sprites.Add(bitmap);
            }
        }

        public void Init()
        {
            if (App.Current.Settings.Grid.Count == 0)
            {
                int n = 0;
                for (int i = 0; i < App.Current.Settings.FieldHeight; i++)
                {
                    if (i == 0 || i == App.Current.Settings.FieldHeight - 1)
                    {
                        for (int j = 0; j < App.Current.Settings.FieldWidth; j++)
                        {
                            App.Current.Settings.Grid.Add(-1);
                        }
                    }
                    else
                    {
                        App.Current.Settings.Grid.Add(-1);
                        for (int j = 0; j < App.Current.Settings.FieldWidth - 2; j++)
                        {
                            App.Current.Settings.Grid.Add(n);
                            n++;
                            if (n > 35) n = 0;
                        }
                        App.Current.Settings.Grid.Add(-1);
                    }
                }
                SortGrid();
            }
           
            FirstSelect = -1;
            SecondSelect = -1;

        }

        void SortGrid()
        {
            //var dialog = new MessageDialog("sort");
            //dialog.ShowAsync();
            //return;
            _pathList.Clear();
            _curPath = null;
            int numIterations = 5;

            do
            {
                Debug.WriteLine("Sort");
                for (int i = 0; i < App.Current.Settings.Grid.Count * 1; i++)
                {
                    int n;
                    do
                    {
                        n = App.Current.RandomGenerator.Next(App.Current.Settings.Grid.Count);
                    } while (App.Current.Settings.Grid[n] == -1);

                    int n2;
                    do{
                        n2 = App.Current.RandomGenerator.Next(App.Current.Settings.Grid.Count);
                    } while (App.Current.Settings.Grid[n2] == -1);

                    int temp = App.Current.Settings.Grid[n];
                    App.Current.Settings.Grid[n] = App.Current.Settings.Grid[n2];
                    App.Current.Settings.Grid[n2] = temp;
                }

                numIterations--;
                if (numIterations < 0)
                {
                    _gameEnd = true;
                    break;
                }
            } while (!ChekGrid());

            if (_gameEnd)
            {
                OnEndGame(new EventArgs());
            }
        }

        bool ChekGrid()
        {
            var result = Solver.ChekGrid();
            //ShowHelp();

            return result;
        }

        bool IsGridEmpty()
        {
            return App.Current.Settings.Grid.All(t => t == -1);
        }

        void DrawPatch()
        {

            //выберем самый коротки
            var orderPathList = from item in _pathList
                                orderby item.Count
                                select item;

            _curPath = orderPathList.First();

            for (int i = 0; i < _curPath.Count; i++)
            {
                var img = new Image {Source = _sprites[47], Stretch = Stretch.Fill};
                ((Grid) GetBorder(_curPath[i].Index).Child).Children.Add(img);
            }
            
        }

        void ClearPath()
        {
            
            if (_curPath == null) return;

            for (int i = 0; i < _curPath.Count; i++)
            {
                var children = ((Grid) GetBorder(_curPath[i].Index).Child).Children;
                if (children.Count > 0 && App.Current.Settings.Grid[_curPath[i].Index] == -1)
                {
                    for (int j = 0; j < children.Count(); j++)
                    {
                        if(children[j] is Image)
                        {
                            children.RemoveAt(j);
                            break;
                        }
                    }
                    //children.RemoveAt(0);
                }
            }
            _pathList.Clear();
            _curPath = null;
        }

        public void Grid_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        private void Border_Tap(object sender, TappedRoutedEventArgs e)
        {
            
            var index = (int)((Border) sender).Tag;
            if (App.Current.Settings.Grid[index] == -1) return;

            //int row = index / Settings.FieldWidth;
            //int col = index - row * Settings.FieldWidth;
            var border = (Border)sender;

            //if (_backgroundColor == Colors.White)
            //{
            //    _backgroundColor = ((SolidColorBrush)border.Background).Color;
            //}


            ((Border) sender).Background = new SolidColorBrush(Colors.Red);
            _selectedBorderList.Add(sender as Border);

            if (FirstSelect == -1)
            {                
                if (border != null) FirstSelect = (int)border.Tag;
            }
            else if(SecondSelect == -1)
            {
                bool timer = false;
                
                if (border != null) SecondSelect = (int)border.Tag;

                if (App.Current.Settings.Grid[FirstSelect] != App.Current.Settings.Grid[SecondSelect] || FirstSelect == SecondSelect)
                {

                }                
                else
                {
                    var newPath = Solver.PathFromTo(FirstSelect, SecondSelect);
                    if (!newPath.IsNull)
                    {
                        _pathList.Add(newPath);
                        DrawPatch();
                        App.Current.Settings.Player1Score += 20;
                        _myDispatcherTimer.Start();
                        timer = true;
                    }
                }

                if (!timer)
                {
                    SetSelectedBordersColor(_backgroundColor);
                    _selectedBorderList.Clear();
                    FirstSelect = -1;
                    SecondSelect = -1;

                    bool temp = ChekGrid();
                    
                    if (IsGridEmpty())
                    {
                        //end
                       
                        OnNextLevel(new EventArgs());
                        return;
                    }

                    if (!temp)
                    {
                        SortGrid();
                        GenerateField();
                    }
                    FirstSelect = -1;
                    SecondSelect = -1;
                } 
            }

            
            
        }


        Border GetBorder(int index)
        {
            return (from control in LayoutRoot.Children.OfType<Border>() where control.Tag.ToString() == index.ToString() select control).FirstOrDefault();
        }

        public void SetSelectedBordersColor(Color color)
        {
            foreach (Border border in _selectedBorderList)
            {
                border.Background = new SolidColorBrush(color);
            }
        }

        void UpColumn()
        {
            int row1 = FirstSelect / App.Current.Settings.FieldWidth;
            int col1 = FirstSelect - row1 * App.Current.Settings.FieldWidth;
            int row2 = SecondSelect / App.Current.Settings.FieldWidth;
            int col2 = SecondSelect - row2 * App.Current.Settings.FieldWidth;

            //up col1
            for (int i = 1; i < App.Current.Settings.FieldHeight - 2; i++)
            {
                int index = col1 + App.Current.Settings.FieldWidth * i;
                if (App.Current.Settings.Grid[index] != -1) continue;

                for (int j = i; j < App.Current.Settings.FieldHeight - 1; j++)
                {
                    int index2 = col1 + App.Current.Settings.FieldWidth * j;
                    int index3 = col1 + App.Current.Settings.FieldWidth * (j + 1);
                    App.Current.Settings.Grid[index2] = App.Current.Settings.Grid[index3];

                }
            }

            //up col2
            for (int i = 1; i < App.Current.Settings.FieldHeight - 2; i++)
            {
                int index = col2 + App.Current.Settings.FieldWidth * i;
                if (App.Current.Settings.Grid[index] != -1) continue;

                for (int j = i; j < App.Current.Settings.FieldHeight - 1; j++)
                {
                    int index2 = col2 + App.Current.Settings.FieldWidth * j;
                    int index3 = col2 + App.Current.Settings.FieldWidth * (j + 1);

                    App.Current.Settings.Grid[index2] = App.Current.Settings.Grid[index3];

                }
            }

            GenerateField();

        }

        void DownColumn()
        {
            int row1 = FirstSelect / App.Current.Settings.FieldWidth;
            int col1 = FirstSelect - row1 * App.Current.Settings.FieldWidth;
            int row2 = SecondSelect / App.Current.Settings.FieldWidth;
            int col2 = SecondSelect - row2 * App.Current.Settings.FieldWidth;

            //up col1
            for (int i = App.Current.Settings.FieldHeight - 2; i > 1; i--)
            {
                int index = col1 + App.Current.Settings.FieldWidth * i;
                if (App.Current.Settings.Grid[index] != -1) continue;

                for (int j = i; j >0 ; j--)
                {
                    int index2 = col1 + App.Current.Settings.FieldWidth * j;
                    int index3 = col1 + App.Current.Settings.FieldWidth * (j - 1);
                    App.Current.Settings.Grid[index2] = App.Current.Settings.Grid[index3];

                }
            }

            //up col2
            for (int i = App.Current.Settings.FieldHeight - 2; i > 1; i--)
            {
                int index = col2 + App.Current.Settings.FieldWidth * i;
                if (App.Current.Settings.Grid[index] != -1) continue;

                for (int j = i; j > 0; j--)
                {
                    int index2 = col2 + App.Current.Settings.FieldWidth * j;
                    int index3 = col2 + App.Current.Settings.FieldWidth * (j - 1);
                    App.Current.Settings.Grid[index2] = App.Current.Settings.Grid[index3];

                }
            }

            GenerateField();

        }

        void LeftRow()
        {
            int row1 = FirstSelect / App.Current.Settings.FieldWidth;
            int row2 = SecondSelect / App.Current.Settings.FieldWidth;

            //up row1
            for (int i = 1; i < App.Current.Settings.FieldWidth - 2; i++)
            {
                int index = i + App.Current.Settings.FieldWidth * row1;
                if (App.Current.Settings.Grid[index] != -1) continue;

                for (int j = i; j < App.Current.Settings.FieldWidth - 1; j++)
                {
                    int index2 = j + App.Current.Settings.FieldWidth * row1;
                    int index3 = (j + 1) + App.Current.Settings.FieldWidth * row1;

                    App.Current.Settings.Grid[index2] = App.Current.Settings.Grid[index3];

                }
            }

            //up row1
            for (int i = 1; i < App.Current.Settings.FieldWidth - 2; i++)
            {
                int index = i + App.Current.Settings.FieldWidth * row2;
                if (App.Current.Settings.Grid[index] != -1) continue;

                for (int j = i; j < App.Current.Settings.FieldWidth - 1; j++)
                {
                    int index2 = j + App.Current.Settings.FieldWidth * row2;
                    int index3 = (j + 1) + App.Current.Settings.FieldWidth * row2;

                    App.Current.Settings.Grid[index2] = App.Current.Settings.Grid[index3];

                }
            }
            GenerateField();
        }

        void RightRow()
        {
            int row1 = FirstSelect / App.Current.Settings.FieldWidth;
            int row2 = SecondSelect / App.Current.Settings.FieldWidth;

            for (int i = App.Current.Settings.FieldWidth - 2; i > 1; i--)
            {
                int index = i + App.Current.Settings.FieldWidth * row1;
                if (App.Current.Settings.Grid[index] != -1) continue;

                for (int j = i; j > 0; j--)
                {
                    int index2 = j + App.Current.Settings.FieldWidth * row1;
                    int index3 = (j - 1) + App.Current.Settings.FieldWidth * row1;
                    App.Current.Settings.Grid[index2] = App.Current.Settings.Grid[index3];

                }
            }

            for (int i = App.Current.Settings.FieldWidth - 2; i > 1; i--)
            {
                int index = i + App.Current.Settings.FieldWidth * row2;
                if (App.Current.Settings.Grid[index] != -1) continue;

                for (int j = i; j > 0; j--)
                {
                    int index2 = j + App.Current.Settings.FieldWidth * row2;
                    int index3 = (j - 1) + App.Current.Settings.FieldWidth * row2;
                    App.Current.Settings.Grid[index2] = App.Current.Settings.Grid[index3];

                }
            }
            GenerateField();
        }

        public void ShowHelp()
        {
            var FirstBorder = GetBorder(Solver.First);
            var SecondBorder = GetBorder(Solver.Second);
            if (FirstBorder != null) FirstBorder.Background = new SolidColorBrush(Colors.Green);
            if (SecondBorder != null) SecondBorder.Background = new SolidColorBrush(Colors.Green);
        }

    }
}