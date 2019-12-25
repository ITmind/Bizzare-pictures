using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizarre_pictures.Code
{
    static class Solver
    {
        public static int First = -1;
        public static int Second = -1;

        static public bool ChekGrid()
        {
            First = -1;
            Second = -1;
            for (int i = 0; i < App.Current.Settings.Grid.Count; i++)
            {
                if (App.Current.Settings.Grid[i] == -1) continue;

                var cell = new Cell(i);                
                var linePath = cell.CalculatePatch();
                if (linePath != null)
                {
                    First = linePath[0].Index;
                    Second = linePath.Last().Index;
                    return true;
                }



                //находим индексы всех подобных картинок
                var indexes = new List<int>();
                for (int i2 = 0; i2 < App.Current.Settings.Grid.Count; i2++)
                {
                    if (App.Current.Settings.Grid[i2] == cell.Value && cell.Index != i2)
                    {
                        indexes.Add(i2);
                    }
                }

                foreach (var index in indexes)
                {
                    var c = new Cell(index);
                    c.CalculatePatch();
                    if (!c.FindIntersection(cell).IsNull)
                    {
                        First = cell.Index;
                        Second = c.Index;
                        return true;
                    }
                }
            }


            return false;
        }

        static public Path PathFromTo(int fromIndex, int toIndex)
        {
            First = -1;
            Second = -1;

            var cell = new Cell(fromIndex);
            var linePath = cell.CalculatePatch();
            if (linePath != null && linePath.Contains(fromIndex) && linePath.Contains(toIndex))
            {
                return linePath;
            }

            var c = new Cell(toIndex);
            c.CalculatePatch();
            var path = c.FindIntersection(cell);
            if (!path.IsNull)
            {
                First = cell.Index;
                Second = c.Index;
                //проставим первую и последнюю точку пути
                path.InsertFirst(First);
                path.Add(Second);
            }

            return path;
        }
    }
}
