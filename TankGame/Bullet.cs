using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Bullet
    {
        // Позиция пули
        public int Row { get; set; }
        public int Col { get; set; }

        // Направление пули
        public Direction Dir { get; private set; }

        // Жива ли пуля, false когда попала в стену, танк или вышла за границу
        public bool IsAlive { get; set; }

        // Владелец пули: true = выпущена игроком, false = выпущена врагом
        public bool IsPlayerBullet { get; private set; }

        public Bullet(int row, int col, Direction dir, bool isPlayerBullet)
        {
            Row = row;
            Col = col;
            Dir = dir;
            IsPlayerBullet = isPlayerBullet;
            IsAlive = true;
        }

        // движение пули
        public void Move()
        {
            switch (Dir)
            {
                case Direction.Up: Row--; break;
                case Direction.Down: Row++; break;
                case Direction.Left: Col--; break;
                case Direction.Right: Col++; break;
            }
        }
    }
}
