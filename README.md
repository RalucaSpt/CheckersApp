# Checkers Game in C#

## Overview
This project implements the classic game of checkers in C# using the Windows Presentation Foundation (WPF) framework, structured around the Model-View-ViewModel (MVVM) design pattern. It features a standard 8x8 board with white and red pieces, following traditional checkers rules.

## Features
- **Two Player Game:** Play as white or red, with red pieces moving first.
- **Graphical User Interface:** A clear and intuitive interface displaying the game board, active player, and piece count.
- **Game States:** Start a new game, save progress, or load a previously saved game.
- **Multiple Jumps:** Configure the game to allow multiple jumps per turn. This setting can be preserved across game sessions.
- **King Pieces:** Pieces reaching the opposite board end become kings, capable of moving and capturing backwards.
- **End Game Detection:** The game concludes when a player loses all pieces, with a victory message displayed.
- **Statistics Tracking:** Record and display the number of games won by each color and the maximum number of pieces remaining at game end.
- **File Handling:** Save game states in a user-defined format and location.

## Menus

### File Menu
- **New Game:** Initializes a new game.
- **Save:** Saves the current game state.
- **Open:** Loads a saved game state.

- **Statistics:** View game statistics.

### Allow Multiple Jumps Button
- **Toggle allowing multiple jumps.**

### Help Menu
- **About:** Information about the game's creator and the project.

## Game Rules
- **Simple Move:** Move one diagonal space forward.
- **Capture:** Jump over an opposing piece diagonally into an open space, removing the jumped piece.
- **Multiple Captures:** If enabled, successive jumps are possible in a single turn.
- **King Transformation:** Pieces reaching the opposite board end are transformed into kings.

## Installation and Setup
1. Ensure Visual Studio is installed with C# and WPF development components.
2. Clone the repository or download the project files.
3. Open the solution file in Visual Studio.
4. Build the project to resolve dependencies.
5. Run the application from Visual Studio or from the executable file.

For more detailed information on gameplay and rules, refer to the help section within the game's menu. Enjoy this robust and intuitive game of checkers!
![checkers](https://github.com/RalucaSpt/CheckersApp/assets/147080664/78d1947d-df23-4be5-875a-734dd68c9275)
