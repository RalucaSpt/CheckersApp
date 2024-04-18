using CheckersApp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersApp.Models
{
    using System.ComponentModel;

    public class Cell : INotifyPropertyChanged
    {
        private bool _isKing;
        public bool IsKing
        {
            get { return _isKing; }
            set
            {
                if (_isKing != value)
                {
                    _isKing = value;
                    OnPropertyChanged(nameof(IsKing));
                }
            }
        }

        private bool _canBeMoved;
        public bool CanBeMoved
        {
            get { return _canBeMoved; }
            set
            {
                if (_canBeMoved != value)
                {
                    _canBeMoved = value;
                    OnPropertyChanged(nameof(CanBeMoved));
                }
            }
        }

        public Position Position { get; set; }

        private Color _color;
        public Color Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged(nameof(Color));
                }
            }
        }

        public Cell(Position position, Color color, bool isKing, bool canBemoved=false)
        {
            Position = position;
            Color = color;
            IsKing = isKing;
        }

        public Cell(Cell cell)
        {
            Position = cell.Position;
            Color = cell.Color;
            IsKing = cell.IsKing;
        }

        public Cell() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
