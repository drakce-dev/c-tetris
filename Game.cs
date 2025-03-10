using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal class Game
    {
        ConsoleColor[,] grid = new ConsoleColor[24, 10];
        public int Score = 0;
        public int Level = 0;
        public int Lines = 0;
        private readonly ConsoleColor emptyCell = ConsoleColor.Black;
        private int blockWidth = 2;
        private int blockHeight = 1;
        private bool canCreateNewPiece = true;
        private Piece? piece;
        private (PieceType? pieceType, bool canSwitch) holdPiece = (null, true);
        private Queue<PieceType> bag = new Queue<PieceType>();
        Random random = new Random();
        public Game()
        {
            for (int i = 0; i < grid.GetLength(0); i++)
                for (int j = 0; j < grid.GetLength(1); j++)
                    grid[i, j] = emptyCell;

            NewBag();
            NewPiece();
        }
        private void PrintBorder()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write('╔');
            Console.Write(new string('═', grid.GetLength(1) * blockWidth));
            Console.Write('╗');

            for (int i = 0; i < grid.GetLength(0) - 2; i++)
            {
                Console.SetCursorPosition(0, Console.CursorTop + 1);
                Console.Write('║');
                Console.SetCursorPosition(grid.GetLength(1) * blockWidth + 1, Console.CursorTop);
                Console.Write('║');
            }
            Console.SetCursorPosition(0, Console.CursorTop + 1);
            Console.Write('╚');
            Console.Write(new string('═', grid.GetLength(1) * blockWidth));
            Console.Write('╝');
        }
        public void PrintNextBorder()
        {
            int left = grid.GetLength(1) * blockWidth + 3;
            Console.SetCursorPosition(left, 0);
            Console.Write("╔═╗");
            for (int i = 0; i < Enum.GetNames(typeof(PieceType)).Length; i++)
            {
                Console.SetCursorPosition(left, Console.CursorTop + 1);
                Console.Write('║');
                Console.SetCursorPosition(left + 2, Console.CursorTop);
                Console.Write('║');
            }
            Console.SetCursorPosition(left, Console.CursorTop + 1);
            Console.Write("╚═╝");
        }
        public void PrintHoldBorder()
        {
            int left = grid.GetLength(1) * blockWidth + 3;
            Console.SetCursorPosition(left, 9);
            Console.Write("╔" + new string('═', blockWidth * 4) + "╗");
            for (int i = 0; i < blockHeight * 4;  i++)
            {
                Console.SetCursorPosition(left, Console.CursorTop + 1);
                Console.Write('║');
                Console.SetCursorPosition(left + blockWidth * 4 + 1, Console.CursorTop);
                Console.Write('║');
            }
            Console.SetCursorPosition(left, Console.CursorTop + 1);
            Console.Write("╚" + new string('═', blockWidth * 4) + "╝");
        }
        public void PrintHold()
        {
            if (holdPiece.pieceType == null) return;
            int left = grid.GetLength(1) * blockWidth + 4;
            Piece print = new Piece((PieceType)holdPiece.pieceType);
            print.SetPivot(new System.Drawing.Point(1, 0));
            Console.SetCursorPosition(left, 10);
            for (int i = 0; i < 2; i++)
            {
                for (int h = 0; h < blockHeight; h++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Console.BackgroundColor = emptyCell;
                        if ((j == 0 || j == 3) && (holdPiece.pieceType != PieceType.I && holdPiece.pieceType != PieceType.O))
                        {
                            Console.Write(new string(' ', 1));
                            if (j == 3) continue;
                        }
                        if (print.Position.Contains(new Point(j, i)))
                            Console.BackgroundColor = (ConsoleColor)holdPiece.pieceType;
                        Console.Write(new string(' ', blockWidth));
                    }
                    Console.SetCursorPosition(left, Console.CursorTop + 1);
                }
            }
        }
        public void PrintNext(int offset = 0)
        {
            Console.BackgroundColor = emptyCell;
            int left = grid.GetLength(1) * blockWidth + 4;
            Console.SetCursorPosition(left, 1);
            List<PieceType> next = new List<PieceType>(bag);
            for (int i = 0; i < Enum.GetNames(typeof(PieceType)).Length; i++)
            {
                Console.ForegroundColor = (ConsoleColor)next[i + offset];
                Console.Write(next[i + offset]);
                Console.SetCursorPosition(left, Console.CursorTop + 1);
            }
        }
        public void Print()
        {
            PrintBorder();
            PrintNextBorder();
            PrintHoldBorder();
            PrintNext();
            PrintHold();
            PrintState();
        }
        public void PrintState()
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < grid.GetLength(0) - 2; i++)
            {
                for (int h = 0; h < blockHeight; h++)
                {
                    Console.SetCursorPosition(1, Console.CursorTop + 1);
                    for (int j = 0; j < grid.GetLength(1); j++)
                    {
                        Console.BackgroundColor = grid[i + 2, j];
                        Console.Write(new string(' ', blockWidth));
                    }
                }
            }

            if (holdPiece.pieceType != null)
                PrintHold();
            if (piece == null)
            {
                PrintNext(1);
                return;
            }

            int bottom = 0;
            //int min = piece.Position.Min(position => position.Y);
            int max = grid.GetLength(0) - piece.Position.Max(position => position.Y) - 1;
            while (bottom < max)
            {
                if (piece.Position.Any(position => IsOccupied(position.Y + bottom + 1, position.X)))
                    break;
                bottom++;
            }
            for (int i = 0; i < piece.Position.Length; i++)
            {
                int top = piece.Position[i].Y - 2 + bottom;
                if (top < 0) continue;
                top = top * blockHeight + 1;
                int left = piece.Position[i].X * blockWidth + 1;
                for (int h = 0; h < blockHeight; h++)
                {
                    Console.SetCursorPosition(left, top + h);
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.Write(new string(' ', blockWidth));
                }
            }

            for (int i = 0; i < piece.Position.Length; i++)
            {
                int top = piece.Position[i].Y - 2;
                if (top < 0) continue;
                top = top * blockHeight + 1;
                int left = piece.Position[i].X * blockWidth + 1;
                for (int h = 0; h < blockHeight; h++)
                {
                    Console.SetCursorPosition(left, top + h);
                    Console.BackgroundColor = piece.Color;
                    Console.Write(new string(' ', blockWidth));
                }
            }
        }
        public void NextState(bool moveDown = true)
        {
            if (piece == null)
            {
                Lines += ClearLines();
                Level = Lines % 10;
                holdPiece.canSwitch = true;
                NewPiece();
                MoveDown();
                PrintNext();
            }
            else if (moveDown)
            {
                MoveDown();
                if (piece == null)
                    NextState();
            }
        }
        public bool Hold()
        {
            if (piece == null) return false;
            if (!holdPiece.canSwitch) return false;
            PieceType pieceType = piece.Type;
            if (holdPiece.pieceType == null) piece = null;
            else NewPiece((PieceType)holdPiece.pieceType);
            holdPiece.pieceType = pieceType;
            holdPiece.canSwitch = false;
            PrintHold();
            return true;
        }
        public int ClearLines()
        {
            int lines = 0;
            for (int i = grid.GetLength(0) - 1; i >= 0; i--)
            {
                if (!Enumerable.Range(0, grid.GetLength(1)).Any(j => grid[i, j] == emptyCell))
                {
                    Array.Copy(grid, 0, grid, grid.GetLength(1), i * grid.GetLength(1));
                    lines++;
                    i++;
                }
            }
            return lines;
        }
        public bool IsOccupied(int row, int column)
        {
            if (row < 0 || row >= grid.GetLength(0) || column < 0 || column >= grid.GetLength(1))
                return true;
            return grid[row, column] != emptyCell;
        }
        public bool IsGameOver()
        {
            return !canCreateNewPiece;
        }
        public PieceType NewPiece()
        {
            PieceType pieceType = bag.Dequeue();
            NewPiece(pieceType);
            if (bag.Count < 7) NewBag();
            return pieceType;
        }
        public void NewPiece(PieceType pieceType)
        {
            piece = new Piece(pieceType);
            piece.SetPivot(new System.Drawing.Point(grid.GetLength(1) / 2 - 1, 0));
            if (piece.Position.Any(point => IsOccupied(point.Y, point.X))) canCreateNewPiece = false;
        }
        private void NewBag()
        {
            List<PieceType> possiblePieces = [PieceType.I, PieceType.O, PieceType.T, PieceType.S, PieceType.Z, PieceType.J, PieceType.L ];
            while (possiblePieces.Count > 0)
            {
                int i = random.Next(possiblePieces.Count);
                bag.Enqueue(possiblePieces[i]);
                possiblePieces.RemoveAt(i);
            }
        }
        public bool Rotate()
        {
            if (piece == null) return false;
            Piece rotatePiece = new Piece(piece.Type);
            rotatePiece.SetPosition(piece.Position);
            rotatePiece.Rotate();
            if (rotatePiece.Position.Any(position => IsOccupied(position.Y, position.X)))
                return false;
            piece.Rotate();
            return true;
        }
        public bool FallDown()
        {
            if (piece == null) return false;
            while (MoveDown()) { }
            return true;
        }
        public bool MoveDown()
        {
            if (piece == null) return false;
            if (piece.Position.Any(point => IsOccupied(point.Y + 1, point.X)))
            {
                for (int i = 0; i < piece.Position.Length; i++)
                {
                    grid[piece.Position[i].Y, piece.Position[i].X] = piece.Color;
                    //grid[piece.Position[i].Y, piece.Position[i].X] = piece.Block;
                }
                piece = null;
                //NewPiece();
                return false;
            }
            piece.MoveDown();
            return true;
        }
        public bool MoveLeft()
        {
            if (piece == null) return false;
            if (piece.Position.Any(point => IsOccupied(point.Y, point.X - 1)))
                return false;
            piece.MoveLeft();
            return true;
        }
        public bool MoveRight()
        {
            if (piece == null) return false;
            if (piece.Position.Any(point => IsOccupied(point.Y, point.X + 1)))
                return false;
            piece.MoveRight();
            return true;
        }
    }
}
