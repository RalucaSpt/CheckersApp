using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CheckersApp.Commands;
using CheckersApp.Models;
using CheckersApp.Models.CheckersApp.Models;
using CheckersApp.ViewModel;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace CheckersApp.ViewModels
{
    public class GameVM : ViewModelBase
    {
        public ICommand CellClickCommand => new RelayCommand(CellClick);
        public ICommand NewGameCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand AboutCommand { get; private set; }
        public ICommand StatisticsCommand { get; private set; }
        public ICommand ToggleMultipleJumpCommand { get; private set; }

        private Game game;
        public int RedPieces
        {
            get { return game.RedPieces; }
            set
            {
                game.RedPieces = value;
                OnPropertyChanged(nameof(RedPieces));
            }
        }
        public int BlackPieces
        {
            get { return game.BlackPieces; }
            set
            {
                game.BlackPieces = value;
                OnPropertyChanged(nameof(BlackPieces));
            }
        }
        public Color Winner
        {
            get { return game.Winner; }
            set
            {
                game.Winner = value;
                OnPropertyChanged(nameof(Winner));
            }
        }
        public Color PlayerTurn
        {
            get { return game.PlayerTurn; }
            set
            {
                game.PlayerTurn = value;
                OnPropertyChanged(nameof(PlayerTurn));
            }
        }
        public bool AllowMultipleJumps
        {
            get { return game.AllowMultipleJumps; }
            set
            {
                game.AllowMultipleJumps = value;
                OnPropertyChanged(nameof(AllowMultipleJumps));
            }
        }
        public bool GameHasStarted
        {
            get { return game.GameHasStarted; }
            set
            {
                game.GameHasStarted = value;
                OnPropertyChanged(nameof(GameHasStarted));
            }
        }

        private ObservableCollection<ObservableCollection<CellVM>> board;
        public ObservableCollection<ObservableCollection<CellVM>> Board
        {
            get => board;
            set
            {
                if (board != value)
                {
                    board = value;
                    OnPropertyChanged(nameof(Board));
                }
            }
        }
        private CellVM _selectedCellVM;
        public CellVM SelectedCell
        {
            get => _selectedCellVM;
            set
            {
                if (_selectedCellVM != value)
                {
                    if (_selectedCellVM != null) _selectedCellVM.IsSelected = false;
                    _selectedCellVM = value;
                    if (_selectedCellVM != null) _selectedCellVM.IsSelected = true;
                    OnPropertyChanged(nameof(SelectedCell));
                }
            }
        }

        public GameVM()
        {
            ToggleMultipleJumpCommand = new RelayCommand(ToggleMultipleJump);
            NewGameCommand = new RelayCommand(NewGame);
            OpenCommand = new RelayCommand(Open);
            SaveCommand = new RelayCommand(Save);
            AboutCommand = new RelayCommand(About);
            StatisticsCommand = new RelayCommand(ShowStatistics);
            game = new Game();
            InitializeBoard();
        }

        private void CellClick(object parameter)
        {
            if (game.Winner != Color.Transparent)
            {
                MessageBox.Show("Jocul s-a terminat! Câștigător: " + game.Winner, "Joc terminat", MessageBoxButton.OK, MessageBoxImage.Information);
                GameStatistics.EndGame(game.Winner, game.GetPiecesRemaining());
                return;
            }
            if (game.isDraw())
            {
                MessageBox.Show("Jocul s-a terminat la egalitate!", "Joc terminat", MessageBoxButton.OK, MessageBoxImage.Information);
                GameStatistics.EndGame(Color.Transparent, game.GetPiecesRemaining());
                return;
            }
            var cellVM = parameter as CellVM;
            if (cellVM == null) return;

            if (game.MustContinueJumping)
            {
                HandleContinuedJumping(cellVM);
            }
            else
            {
                HandleNewSelectionOrMove(cellVM);
            }
            RefreshBoard();
            OnPropertyChanged(nameof(RedPieces));
            OnPropertyChanged(nameof(BlackPieces));
        }

        private void HandleContinuedJumping(CellVM cellVM)
        {
            if (SelectedCell == null || cellVM != SelectedCell)
            {
                if (game.isValidCapture(SelectedCell.Model, cellVM.Model))
                {
                    game.TryPerformCapture(SelectedCell.Model, cellVM.Model);
                    CompleteMoveSequence(cellVM);
                }
                else
                {
                    ResetMove();
                }
            }
        }

        private void HandleNewSelectionOrMove(CellVM cellVM)
        {
            if (game.IsValidSelection(cellVM.Model))
            {
                SelectCell(cellVM);
            }
            else if (SelectedCell != null && game.IsValidMove(SelectedCell.Model, cellVM.Model))
            {
                ExecuteMove(cellVM);
            }
        }

        private void CompleteMoveSequence(CellVM cellVM)
        {
            ClearSelections();
            if (game.MustContinueJumping)
            {
                SelectedCell = cellVM;
            }
            else
            {
                TogglePlayerTurn();
            }
        }

        private void ResetMove()
        {
            game.MustContinueJumping = false;
            TogglePlayerTurn();
            ClearSelections();
        }

        private void SelectCell(CellVM cellVM)
        {
            game.ClearPossibleMoves();
            game.GetAllPossibleMoves(cellVM.Model);
            SelectedCell = cellVM;
        }

        private void ExecuteMove(CellVM cellVM)
        {
            if (game.PerformMove(SelectedCell.Model, cellVM.Model))
            {
                if (!game.MustContinueJumping)
                {

                    TogglePlayerTurn();
                    ClearSelections();
                }
                else
                {
                    SelectedCell = cellVM;
                }
            }
        }

        private void ClearSelections()
        {
            if (SelectedCell != null)
            {
                SelectedCell.IsSelected = false;
                SelectedCell.CanBeMoved = false;
                SelectedCell = null;
            }
            game.ClearPossibleMoves();
        }

        public void RefreshBoard()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Board[row][col].Color = game.GetCell(row, col).Color;
                    Board[row][col].IsKing = game.GetCell(row, col).IsKing;
                    Board[row][col].CanBeMoved = game.GetCell(row, col).CanBeMoved;
                }
            }
        }

        private void TogglePlayerTurn()
        {
            game.ChangeTurn();
            OnPropertyChanged(nameof(game.PlayerTurn));

            game.CheckForWin();
            OnPropertyChanged(nameof(Winner));
        }

        private void InitializeBoard()
        {
            Board = new ObservableCollection<ObservableCollection<CellVM>>();
            for (int row = 0; row < 8; row++)
            {
                var newRow = new ObservableCollection<CellVM>();
                for (int col = 0; col < 8; col++)
                {
                    newRow.Add(new CellVM(game.Board[row][col]));
                }
                Board.Add(newRow);
            }
            OnPropertyChanged(nameof(Board));
        }

        public void NewGame()
        {
            game = new Game();
            InitializeBoard();
            OnPropertyChanged(nameof(RedPieces));
            OnPropertyChanged(nameof(BlackPieces));
            OnPropertyChanged(nameof(Winner));
            OnPropertyChanged(nameof(PlayerTurn));
        }

        public void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = "Checkers_WPF_App\\CheckersApp\\CheckersApp\\Json";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                string serializedGameState = File.ReadAllText(fileName);

                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                };

                try
                {
                    game = JsonConvert.DeserializeObject<Game>(serializedGameState, settings);
                    game.Board.RemoveRange(0, 8);
                    RefreshBoard();
                    OnPropertyChanged(nameof(RedPieces));
                    OnPropertyChanged(nameof(BlackPieces));
                    OnPropertyChanged(nameof(Winner));
                    OnPropertyChanged(nameof(PlayerTurn));
                }
                catch (JsonException ex)
                {
                    MessageBox.Show($"This file doesn't contain a game.", "Error");
                }
            }
        }

        public void Save()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*";
            saveFileDialog.InitialDirectory = "Checkers_WPF_App\\CheckersApp\\CheckersApp\\Json"; 
            saveFileDialog.FileName = "MyGameSave.json";

            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                string serializedGameState = JsonConvert.SerializeObject(game);
                File.WriteAllText(fileName, serializedGameState);
            }
        }

        public void About()
        {
            string aboutText = "Creator: Spataru Larisa-Raluca\n" +
                               "Email: larisa.spataru@student.unitbv.ro\n" +
                               "Grupa: 10LF224\n" +
                               "Descriere:\n" +
                               "Jocul de dame este un joc de strategie între doi jucători pe o tablă de 8x8. Fiecare jucător controlează piese albe sau roșii și își mută piesele pe diagonale pentru a captura sau bloca piesele adversarului. \n" +
                               "Scopul este să elimini toate piesele adversarului sau să-l blochezi. Jucătorii se succed în mutări, fiecare putând face o mutare simplă sau să captureze piesele adversarului prin sărituri peste ele. \n" +
                               "Jocul se termină când unul dintre jucători nu mai poate muta sau își pierde toate piesele. Interfața grafică va reflecta starea jocului și va permite utilizatorilor să facă mutări valide, evidențiind numărul de piese rămase pentru fiecare jucător.";
            MessageBox.Show(aboutText, "About");
        }

        private void ShowStatistics()
        {
            GameStatistics.LoadStatistics("Checkers_WPF_App\\CheckersApp\\CheckersApp\\Json\\statistics.json");
            String statistics = "Statistici jocuri:\n" +
                                "Jocuri câștigate de jucătorul cu piesele roșii: " + GameStatistics.RedWins + "\n" +
                                "Jocuri câștigate de jucătorul cu piesele negre: " + GameStatistics.BlackWins + "\n" +
                                "Jocuri terminate la egalitate: " + GameStatistics.Draws + "\n" +
                                "Piese rămase la finalul jocurilor: " + GameStatistics.MaxPiecesRemaining + "\n";
            MessageBox.Show(statistics, "Statistici jocuri");
        }

        private void ToggleMultipleJump()
        {
            if (game.GameHasStarted)
            {
                return;
            }
            AllowMultipleJumps = !AllowMultipleJumps;
        }
    }
}