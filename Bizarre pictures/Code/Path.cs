using System.Collections.Generic;
using System.Linq;

namespace Bizarre_pictures
{
    public class Path:IEnumerable<PatchItem>
    {
        readonly List<PatchItem> _path = new List<PatchItem>();

        public int NumAngle
        {
            get
            {
                int num = 0;
 
                for (int i = 2; i < _path.Count; i++)
                {
                    if (_path[i-2].Col != _path[i].Col && _path[i-2].Row != _path[i].Row)
                    {
                        num++;
                    }
                }

                return num;
            }
        }

        public bool IsNull
        {
            get { return !_path.Any(); }
        }

        /// <summary>
        /// Истина, если этот путь прямой до другого, такого же изображения
        /// </summary>
        public bool Close { get; set; }

        public Path()
        {
            
        }

        /// <summary>
        /// Путь на основе переданых путей
        /// </summary>
        /// <param name="path"></param>
        public Path(params Path[] path):this()
        {
            bool intersect;

            for (int i = 0; i < path.Count(); i++)
            {
                intersect = false;
                foreach (var item in path[i])
                {
                    if(!_path.Any(x=>x.Index == item.Index))
                    {
                        _path.Add(item);
                    }
                    else if(i == path.Count()-1)
                    {
                        //если это последний путь то выход
                        break;
                    }
                    //Если есть пересечение с каким либо другим путем, то переходим к следующему пути
                    for (int j = i+1; j < path.Count(); j++)
                    {
                        if(path[j].Any(it => it.Index == item.Index))
                        {
                            intersect = true;
                            break;
                        }
                    }

                    if(intersect) break;
                }
            }

        }

        public PatchItem this[int key]
        {
            get
            {
                return _path[key];
            }
        }

        public int Count
        {
            get
            {
                return _path.Count;
            }
        }

        public void Add(int index)
        {
            _path.Add(new PatchItem(index));
        }

        public void Add(int row,int col)
        {
            _path.Add(new PatchItem(row,col));
        }

        public void InsertFirst(int index)
        {
            _path.Insert(0,new PatchItem(index));
        }

        public Path Copy()
        {
            var temp = new Path();
            foreach (var item in _path)
            {
                temp.Add(item.Index);
            }
            return temp;
        }

        public bool Contains(int index)
        {
            return _path.Any(item => item.Index == index);
        }

        public IEnumerator<PatchItem> GetEnumerator()
        {
            return _path.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public enum Directional
    {
        Up,
        Left,
        Right,
        Down,
        None
    }
}
