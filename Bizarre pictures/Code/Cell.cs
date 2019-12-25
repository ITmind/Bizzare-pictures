using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizarre_pictures.Code
{
    public class Cell
    {
        public int Value { get; set; }
        public int Index { get; set; }
        public List<Path> Rays { get; set; }

        public Cell(int index)
        {
            Rays = new List<Path>(4);
            Index = index;
            Value = App.Current.Settings.Grid[index];
        }

        /// <summary>
        /// Вычисляем все лучи ячейки
        /// </summary>
        public Path CalculatePatch()
        {
            Rays.Clear();
            Rays.Add(Walk(Index, Directional.Up));
            Rays.Add(Walk(Index, Directional.Right));
            Rays.Add(Walk(Index, Directional.Down));
            Rays.Add(Walk(Index, Directional.Left));
            var r = Rays.FirstOrDefault(x => x.Close);
            if (r != null) return r;
            return null;
        }

        /// <summary>
        /// Показывает направление в котором прямое пересечение луча
        /// </summary>
        /// <returns>Направление</returns>
        public Directional IsIntersection()
        {
            Directional result = Directional.None;
            if (Rays[0] == null)
            {
                result = Directional.Up;
            }
            else if (Rays[1] == null)
            {
                result = Directional.Right;
            }
            else if (Rays[2] == null)
            {
                result = Directional.Down;
            }
            else if (Rays[3] == null)
            {
                result = Directional.Left;
            }
            return result;
        }

        /// <summary>
        /// Строит луч в указаном направлении до препятствия
        /// </summary>
        /// <param name="fromIndex">Ячейка, от которой строим луч</param>
        /// <param name="directional">Направление луча</param>
        /// <returns>Путь</returns>
        Path Walk(int fromIndex, Directional directional)
        {
            var path = new Path();
            int row = fromIndex / App.Current.Settings.FieldWidth;
            int col = fromIndex - row * App.Current.Settings.FieldWidth;

            switch (directional)
            {
                case Directional.Down:
                    for (row = row+1; row < App.Current.Settings.FieldHeight; row++)
                    {
                        int index = col + App.Current.Settings.FieldWidth * row;

                        if (App.Current.Settings.Grid[index] != -1)
                        {
                            if (App.Current.Settings.Grid[fromIndex] == App.Current.Settings.Grid[index])
                            {
                                path.InsertFirst(fromIndex);
                                path.Add(index);
                                path.Close = true;
                                return path;
                            }
                            break;
                        }

                        path.Add(index);
                    }

                    break;
                case Directional.Left:
                    for (col=col-1; col > -1; col--)
                    {
                        int index = col + App.Current.Settings.FieldWidth * row;

                        if (App.Current.Settings.Grid[index] != -1)
                        {
                            if (App.Current.Settings.Grid[fromIndex] == App.Current.Settings.Grid[index])
                            {
                                path.InsertFirst(fromIndex);
                                path.Add(index);
                                path.Close = true;
                                return path;
                            }
                            break;
                        }

                        path.Add(index);
                    }
                    break;
                case Directional.Right:
                    for (col = col +1 ; col < App.Current.Settings.FieldWidth; col++)
                    {
                        int index = col + App.Current.Settings.FieldWidth * row;

                        if (App.Current.Settings.Grid[index] != -1)
                        {
                            if (App.Current.Settings.Grid[fromIndex] == App.Current.Settings.Grid[index])
                            {
                                path.InsertFirst(fromIndex);
                                path.Add(index);
                                path.Close = true;
                                return path;
                            }
                            break;
                        }

                        path.Add(index);
                    }
                    break;
                case Directional.Up:
                    for (row = row - 1; row > -1; row--)
                    {
                        int index = col + App.Current.Settings.FieldWidth * row;

                        if (App.Current.Settings.Grid[index] != -1)
                        {
                            if (App.Current.Settings.Grid[fromIndex] == App.Current.Settings.Grid[index])
                            {
                                path.InsertFirst(fromIndex);
                                path.Add(index);
                                path.Close = true;
                                return path;
                            }
                            break;
                        }

                        path.Add(index);
                    }
                    break;
            }


            return path;
        }

        /// <summary>
        /// Определяет, свободен ли путь
        /// </summary>
        /// <param name="from">Начальная ячейка</param>
        /// <param name="to">Конечна ячейка</param>
        /// <returns>путь</returns>
        Path IsPathFree(PatchItem from, PatchItem to)
        {
            var path = new Path();
            if (from == null || to == null) return path;         

            if (from.Col < to.Col)
            {
                for (int i = from.Col; i <= to.Col; i++)
                {
                    int index = i + App.Current.Settings.FieldWidth * from.Row;
                    if (App.Current.Settings.Grid[index] != -1) return new Path();
                    path.Add(index);
                }
            }
            else if (from.Col > to.Col)
            {
                for (int i = from.Col; i >= to.Col; i--)
                {
                    int index = i + App.Current.Settings.FieldWidth * from.Row;
                    if (App.Current.Settings.Grid[index] != -1) return new Path();
                    path.Add(index);
                }
            }
            else if (from.Row < to.Row)
            {
                for (int i = from.Row; i <= to.Row; i++)
                {
                    int index = from.Col + App.Current.Settings.FieldWidth * i;
                    if (App.Current.Settings.Grid[index] != -1) return new Path();
                    path.Add(index);
                }
            }
            else if (from.Row > to.Row)
            {
                for (int i = from.Row; i >= to.Row; i--)
                {
                    int index = from.Col + App.Current.Settings.FieldWidth * i;
                    if (App.Current.Settings.Grid[index] != -1) return new Path();
                    path.Add(index);
                }
            }

            return path;
        }

        /// <summary>
        /// Находим путь
        /// </summary>
        /// <param name="ray">Луч, для которого ищем пересечение с даной ячейкой</param>
        /// <returns>Путь пересечения</returns>
        Path FindIntersection(Path ray)
        {
            var path = new Path();
            if (ray == null || ray.IsNull || ray.Close) return path;

            //если есть явное пересечение лучей то нашли
            foreach (PatchItem item1 in ray)
            {
                foreach (Path x1 in Rays)
                {
                    if (x1 != null && !x1.IsNull)
                    {
                        //получим точку пересечения
                        PatchItem upI = x1.FirstOrDefault(x => x.Col == item1.Col && x.Row == item1.Row);
                        if (upI != null) return new Path(ray,x1);
                    }
                }
            }

            //если нет, то находим одинаковые колонки или строки и строим дополнительный луч
            foreach (var item in ray)
            {
                foreach (var cellRay in Rays)
                {
                    //находим точки пересечения текущего луча с переданным
                    var rayItems = cellRay.Where(x => x.Col == item.Col || x.Row == item.Row);
                    //строим пути от каждой точки
                    foreach (var rayItem in rayItems)
                    {
                        var patch = IsPathFree(item, rayItem);
                        if (!patch.IsNull) return new Path(ray, patch, cellRay);
                    }
                }                                
            }           

            return path;
        }

        public Path FindIntersection(Cell cell)
        {
            foreach (var ray in cell.Rays)
            {
                var patch = FindIntersection(ray);
                if (!patch.IsNull) return patch;
            }            

            return new Path();
        }
    }
}
