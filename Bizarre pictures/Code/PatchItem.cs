using Windows.UI.Xaml;

namespace Bizarre_pictures
{
    public class PatchItem
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int Index
        {
            get
            {

                return Col + App.Current.Settings.FieldWidth * Row;
            }
        }
        public PatchItem(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public PatchItem(int index)
        {
            Row = index / App.Current.Settings.FieldWidth;
            Col = index - Row * App.Current.Settings.FieldWidth;
        }
    }
}
