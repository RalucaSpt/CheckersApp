using CheckersApp.Models.CheckersApp.Models;
using CheckersApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CheckersApp.Models
{
    public class Game
    {
        public List<List<Cell>> Board { get; set; }
        public Color PlayerTurn { get; set; }
        public int RedPieces { get; set; }
        public int BlackPieces { get; set; }
        public Color Winner { get; set; }
        public List<Position> PossibleMoves { get; set; }
        public bool AllowMultipleJumps = false;
        public bool MustContinueJumping { get; set; } = false;
        public bool GameHasStarted { get; set; } = false;
        public Cell SelectedCell { get; set; }

        public Game()
        {
            InitBoard();
            PlayerTurn = Color.Red;
            RedPieces = 12;
            BlackPieces = 12;
            Winner = Color.Transparent;
            PossibleMoves = new List<Position>();
            GameHasStarted = false;
            AllowMultipleJumps = false;
        }

        public Game(Game g2)
        {
            this.PlayerTurn = g2.PlayerTurn;
            this.RedPieces = g2.RedPieces;
            this.BlackPieces = g2.BlackPieces;
            this.Winner = g2.Winner;
            this.MustContinueJumping = g2.MustContinueJumping;
            this.GameHasStarted = g2.GameHasStarted;
            this.SelectedCell = g2.SelectedCell;
            this.PossibleMoves = new List<Position>();
            for (int i = 0; i < g2.PossibleMoves.Count; i++)
            {
                this.PossibleMoves.Add(new Position(g2.PossibleMoves[i].Row, g2.PossibleMoves[i].Column));
            }
            this.Board = new List<List<Cell>>();
            for (int i = 0; i < g2.Board.Count; i++)
            {
                List<Cell> row = new List<Cell>();
                for (int j = 0; j < g2.Board[i].Count; j++)
                {
                    row.Add(new Cell(g2.Board[i][j]));
                }
                this.Board.Add(row);
            }
        }

        public void InitBoard()
        {
            Board = new List<List<Cell>>();
            for (int row = 0; row < 8; row++)
            {
                var newRow = new List<Cell>();

                for (int column = 0; column < 8; column++)
                {
                    if (row % 2 != column % 2)
                    {
                        if (row < 3)
                        {
                            newRow.Add(new Cell(new Position(row, column), Color.Red, false));
                        }
                        else if (row > 4)
                        {
                            newRow.Add(new Cell(new Position(row, column), Color.Black, false));
                        }
                        else
                        {
                            newRow.Add(new Cell(new Position(row, column), Color.Transparent, false));
                        }
                    }
                    else
                    {
                        newRow.Add(new Cell(new Position(row, column), Color.Transparent, false));
                    }
                }

                Board.Add(newRow);
            }
        }

        public Cell GetCell(int row, int column)
        {
            return Board[row][column];
        }

        public bool IsValidSelection(Cell cell)
        {
            return cell != null && cell.Color == PlayerTurn;
        }

        public bool IsValidMove(Cell fromCell, Cell toCell)
        {
            if (fromCell == null || toCell == null) return false;
            if (toCell.Color != Color.Transparent) return false;

            int rowDiff = Math.Abs(toCell.Position.Row - fromCell.Position.Row);
            int colDiff = Math.Abs(toCell.Position.Column - fromCell.Position.Column);
            if (rowDiff != 1 && rowDiff != 2) return false;
            if (rowDiff != colDiff) return false;

            return fromCell.IsKing || (fromCell.Color == Color.Red && toCell.Position.Row > fromCell.Position.Row) ||
                   (fromCell.Color == Color.Black && toCell.Position.Row < fromCell.Position.Row);
        }

        public void CheckForWin()
        {
            if (RedPieces == 0)
            {
                Winner = Color.Black;
            }
            if (BlackPieces == 0)
            {
                Winner = Color.Red;
            }
            if (Winner != Color.Transparent)
            {
                string winnerMessage = $"Câștigătorul este: {Winner}. Felicitări!";
                MessageBox.Show(winnerMessage, "Joc terminat", MessageBoxButton.OK, MessageBoxImage.Information);
                GameStatistics.EndGame(Winner, GetPiecesRemaining());
            }
        }

        public bool HasValidMove()
        {
            for (int row = 0; row < Board.Count; row++)
            {
                for (int col = 0; col < Board[row].Count; col++)
                {
                    Cell cell = Board[row][col];
                    if (cell.Color == PlayerTurn)
                    {
                        if (CanMove(cell) || CanCapture(cell))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool CanMove(Cell cell)
        {
            int rowDirection = cell.Color == Color.Black ? 1 : -1;
            if (cell.IsKing) rowDirection = 0;

            foreach (int direction in new int[] { -1, 1 })
            {
                int newRow = cell.Position.Row + (rowDirection == 0 ? direction : rowDirection);
                foreach (int colOffset in new int[] { -1, 1 })
                {
                    int newCol = cell.Position.Column + colOffset;
                    if (IsValidPosition(newRow, newCol) && Board[newRow][newCol].Color == Color.Transparent)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CanCapture(Cell cell)
        {
            int[] possibleJumps;
            if (cell.IsKing)
            {
                possibleJumps = new int[] { -2, 2 };
            }
            else
            {
                possibleJumps = cell.Color == Color.Black ? new int[] { -2 } : new int[] { 2 };
            }

            foreach (int rowJump in possibleJumps)
            {
                foreach (int colJump in new int[] { -2, 2 })
                {
                    int newRow = cell.Position.Row + rowJump;
                    int newCol = cell.Position.Column + colJump;
                    if (IsValidPosition(newRow, newCol))
                    {
                        int midRow = (cell.Position.Row + newRow) / 2;
                        int midCol = (cell.Position.Column + newCol) / 2;
                        Cell targetCell = Board[newRow][newCol];
                        Cell betweenCell = Board[midRow][midCol];

                        if (targetCell.Color == Color.Transparent && betweenCell.Color != Color.Transparent && betweenCell.Color != cell.Color)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < Board.Count && col >= 0 && col < Board[row].Count;
        }

        public bool PerformMove(Cell fromCellVM, Cell toCellVM)
        {
            if (Math.Abs(fromCellVM.Position.Row - toCellVM.Position.Row) == 2)
            {
                int capturedRow = (fromCellVM.Position.Row + toCellVM.Position.Row) / 2;
                int capturedCol = (fromCellVM.Position.Column + toCellVM.Position.Column) / 2;
                Cell capturedCellVM = Board[capturedRow][capturedCol];

                if (TryPerformCapture(fromCellVM, toCellVM)) return true;
            }
            else
            {
                SimpleMove(fromCellVM, toCellVM);
                return true;
            }
            return false;
        }

        public bool TryPerformCapture(Cell fromCellVM, Cell toCellVM)
        {
            int capturedRow = (fromCellVM.Position.Row + toCellVM.Position.Row) / 2;
            int capturedCol = (fromCellVM.Position.Column + toCellVM.Position.Column) / 2;
            Cell capturedCellVM = Board[capturedRow][capturedCol];

            if (capturedCellVM.Color != PlayerTurn && capturedCellVM.Color != Color.Transparent)
            {
                capturedCellVM.Color = Color.Transparent;
                SimpleMove(fromCellVM, toCellVM);

                if (PlayerTurn == Color.Black) RedPieces--;
                else BlackPieces--;

                MustContinueJumping = CanCapture(Board[toCellVM.Position.Row][toCellVM.Position.Column]);
                if (!AllowMultipleJumps)
                    MustContinueJumping = false;
                return true;
            }
            return false;
        }

        public void ChangeTurn()
        {
            PlayerTurn = PlayerTurn == Color.Red ? Color.Black : Color.Red;
        }

        private void SimpleMove(Cell fromCellVM, Cell toCellVM)
        {
            GameHasStarted = true;
            Board[toCellVM.Position.Row][toCellVM.Position.Column].Color = Board[fromCellVM.Position.Row][fromCellVM.Position.Column].Color;
            Board[toCellVM.Position.Row][toCellVM.Position.Column].IsKing = Board[fromCellVM.Position.Row][fromCellVM.Position.Column].IsKing;
            Board[fromCellVM.Position.Row][fromCellVM.Position.Column].IsKing = false;
            Board[fromCellVM.Position.Row][fromCellVM.Position.Column].Color = Color.Transparent;
            CheckForPromotion(Board[toCellVM.Position.Row][toCellVM.Position.Column]);
        }

        private void CheckForPromotion(Cell cell)
        {
            if (!cell.IsKing && ((cell.Color == Color.Black && cell.Position.Row == 0) || (cell.Color == Color.Red && cell.Position.Row == 7)))
            {
                Board[cell.Position.Row][cell.Position.Column].IsKing = true;
            }
        }

        public int GetPiecesRemaining()
        {
            return BlackPieces + RedPieces;
        }

        public void GetAllPossibleMoves(Cell selectedCell)
        {
            int[] directions = selectedCell.IsKing ? new int[] { -1, 1 } : selectedCell.Color == Color.Red ? new int[] { 1 } : new int[] { -1 };

            foreach (int direction in directions)
            {
                for (int dCol = -1; dCol <= 1; dCol += 2)
                {
                    int newRow = selectedCell.Position.Row + direction;
                    int newCol = selectedCell.Position.Column + dCol;

                    if (newRow >= 0 && newRow < 8 && newCol >= 0 && newCol < 8)
                    {
                        if (Board[newRow][newCol].Color == Color.Transparent)
                        {
                            PossibleMoves.Add(new Position { Row = newRow, Column = newCol });
                        }
                        int captureRow = newRow + direction;
                        int captureCol = newCol + dCol;
                        if (captureRow >= 0 && captureRow < 8 && captureCol >= 0 && captureCol < 8
                           && Board[newRow][newCol].Color != Color.Transparent && Board[newRow][newCol].Color != selectedCell.Color
                           && Board[captureRow][captureCol].Color == Color.Transparent)
                        {
                            PossibleMoves.Add(new Position { Row = captureRow, Column = captureCol });
                        }
                    }
                }
            }
            MarkPossibleMoves();
        }

        public void MarkPossibleMoves()
        {
            if (PossibleMoves.Count == 0) return;
            foreach (var move in PossibleMoves)
            {
                Board[move.Row][move.Column].CanBeMoved = true;
            }
        }

        public void ClearPossibleMoves()
        {
            foreach (var row in Board)
            {
                foreach (var cell in row)
                {
                    cell.CanBeMoved = false;
                }
            }
            PossibleMoves.Clear();
        }

        public bool isValidCapture(Cell fromCell, Cell toCell)
        {
            if (Math.Abs(fromCell.Position.Row - toCell.Position.Row) != 2) return false;
            int rowJump = (toCell.Position.Row - fromCell.Position.Row) / 2;
            int colJump = (toCell.Position.Column - fromCell.Position.Column) / 2;
            int midRow = fromCell.Position.Row + rowJump;
            int midCol = fromCell.Position.Column + colJump;
            Cell midCell = Board[midRow][midCol];
            return midCell.Color != Color.Transparent && midCell.Color != fromCell.Color && toCell.Color == Color.Transparent;
        }

        public bool isDraw()
        {

            for (int i = 0; i < Board.Count; i++)
            {
                for (int j = 0; j < Board[i].Count; j++)
                {
                    if (Board[i][j].Color == PlayerTurn)
                    {
                        GetAllPossibleMoves(Board[i][j]);
                    }
                }
            }
            int count = PossibleMoves.Count;
            ClearPossibleMoves();
            return count == 0;
        }

    }
}
