using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal class Piece
    {
        public PieceType Type { get; }
        public ConsoleColor Color { get; }
        public Point[] Position { get; private set; } = new Point[4];
        public Piece(PieceType type)
        {
            Type = type;
            Color = (ConsoleColor)type;
            SetPivot(new Point(0, 0));
        }
        public void SetPosition(Point[] position)
        {
            for (int i = 0; i < position.Length; i++)
                Position[i] = position[i];
        }
        public void SetPivot(Point pivot)
        {
            switch (Type)
            {
                case PieceType.I:
                    Position[0] = new Point(pivot.X, pivot.Y);
                    Position[1] = new Point(pivot.X - 1, pivot.Y);
                    Position[2] = new Point(pivot.X + 1, pivot.Y);
                    Position[3] = new Point(pivot.X + 2, pivot.Y);
                    break;
                case PieceType.O:
                    Position[0] = new Point(pivot.X, pivot.Y);
                    Position[1] = new Point(pivot.X + 1, pivot.Y);
                    Position[2] = new Point(pivot.X, pivot.Y + 1);
                    Position[3] = new Point(pivot.X + 1, pivot.Y + 1);
                    break;
                case PieceType.T:
                    Position[0] = new Point(pivot.X, pivot.Y + 1);
                    Position[1] = new Point(pivot.X - 1, pivot.Y + 1);
                    Position[2] = new Point(pivot.X, pivot.Y);
                    Position[3] = new Point(pivot.X + 1, pivot.Y + 1);
                    break;
                case PieceType.S:
                    Position[0] = new Point(pivot.X, pivot.Y + 1);
                    Position[1] = new Point(pivot.X + 1, pivot.Y);
                    Position[2] = new Point(pivot.X - 1, pivot.Y + 1);
                    Position[3] = new Point(pivot.X, pivot.Y);
                    break;
                case PieceType.Z:
                    Position[0] = new Point(pivot.X, pivot.Y + 1);
                    Position[1] = new Point(pivot.X - 1, pivot.Y);
                    Position[2] = new Point(pivot.X + 1, pivot.Y + 1);
                    Position[3] = new Point(pivot.X, pivot.Y);
                    break;
                case PieceType.J:
                    Position[0] = new Point(pivot.X, pivot.Y + 1);
                    Position[1] = new Point(pivot.X - 1, pivot.Y + 1);
                    Position[2] = new Point(pivot.X + 1, pivot.Y + 1);
                    Position[3] = new Point(pivot.X - 1, pivot.Y);
                    break;
                case PieceType.L:
                    Position[0] = new Point(pivot.X, pivot.Y + 1);
                    Position[1] = new Point(pivot.X - 1, pivot.Y + 1);
                    Position[2] = new Point(pivot.X + 1, pivot.Y + 1);
                    Position[3] = new Point(pivot.X + 1, pivot.Y);
                    break;
            }
        }
        public void MoveDown()
        {
            for (int i = 0; i < Position.Length; i++)
                Position[i].Y += 1;
        }
        public void MoveLeft()
        {
            for (int i = 0; i < Position.Length; i++)
                Position[i].X -= 1;
        }
        public void MoveRight()
        {
            for (int i = 0; i < Position.Length; i++)
                Position[i].X += 1;
        }
        public void Rotate()
        {
            // newX = pivotX + pivotY - oldY
            // newY = pivotY - pivotX + oldX
            if (Type == PieceType.O) return;
            Point pivot = Position[0];
            for (int i = 0; i < Position.Length; i++)
            {
                Position[i] = new Point(
                    pivot.X + pivot.Y - Position[i].Y,
                    pivot.Y - pivot.X + Position[i].X);
            }
        }
    }
}
