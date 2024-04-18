using CheckersApp.Models;
using CheckersApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersApp.ViewModels
{
    public class CellVM : ViewModelBase
    {
        private Cell _cell;
        public CellVM(Cell cell)
        {
            _cell = cell;
            Color = cell.Color;
            IsKing = cell.IsKing;
            Position = cell.Position;
        }

        public Cell Model => _cell;

        public Color Color
        {
            get { return _cell.Color; }
            set
            {
                _cell.Color = value;
                OnPropertyChanged(nameof(Color));
            }
        }

        public bool IsKing
        {
            get { return _cell.IsKing; }
            set
            {
                _cell.IsKing = value;
                OnPropertyChanged(nameof(IsKing));
            }
        }

        public Position Position
        {
            get { return _cell.Position; }
            set
            {
                _cell.Position = value;
                OnPropertyChanged(nameof(Position));
            }
        }
        public bool CanBeMoved
        {
            get { return _cell.CanBeMoved; }
            set
            {
                _cell.CanBeMoved = value;
                OnPropertyChanged(nameof(CanBeMoved));
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public bool SelectCell(Cell oldSelectedCell, CellVM newSelectedCell, Cell selectedCell)
        {
            if (oldSelectedCell == null)
            {
                oldSelectedCell = selectedCell;
                newSelectedCell.IsSelected = true;
                return true;
            }
            if (oldSelectedCell == selectedCell)
            {
                newSelectedCell.IsSelected = false;
                oldSelectedCell = null;
                return true;
            }
            if (oldSelectedCell != selectedCell && selectedCell.Color == newSelectedCell.Color)
            {
                newSelectedCell.IsSelected = true;
                oldSelectedCell = selectedCell;
                return true;
            }

            return false;
        }
    } 
}



