using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public enum CellType
    {
        Empty,      // пустая клетка
        Wall,       // обычная стена (разрушаемая)
        Water,      // вода (пули проходят, танки нет)
    }

    // Структура клетки карты
    public struct Cell
    {
        public CellType Type;     // тип клетки
        public int Hp;            // количество хп у стены (0 = разрушена)
        public char DisplayChar;  // символ для отрисовки
    }

    public class Map
    {
        private Cell[,] _cells;

        public int Width { get; private set; }
        public int Height { get; private set; }

        private const int WallMaxHp = 3; // стена выдерживает 3 попадания

        public Map(string[] mapLines)
        {
            Height = mapLines.Length;
            Width = 0;
            foreach (string line in mapLines)
            {
                if (line.Length > Width) Width = line.Length;
            }

            _cells = new Cell[Height, Width];

            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    char ch = col < mapLines[row].Length ? mapLines[row][col] : ' ';
                    _cells[row, col] = ParseCell(ch);
                }
            }
        }

        private Cell ParseCell(char ch)
        {
            switch (ch)
            {
                case '#':
                    return new Cell { Type = CellType.Wall, Hp = WallMaxHp, DisplayChar = '#' };
                case '~':
                    return new Cell { Type = CellType.Water, Hp = 0, DisplayChar = '~' };
                default:
                    return new Cell { Type = CellType.Empty, Hp = 0, DisplayChar = ' ' };
            }
        }

        // можно ли танку пройти в клетку
        public bool IsPassable(int row, int col)
        {
            // Проверка выхода за границы
            if (row < 0 || row >= Height || col < 0 || col >= Width) return false;

            // Танк не может ехать через стены и воду
            return _cells[row, col].Type == CellType.Empty;
        }

        // может ли пуля пролететь через клетку
        public bool IsBulletPassable(int row, int col)
        {
            if (row < 0 || row >= Height || col < 0 || col >= Width) return false;

            CellType type = _cells[row, col].Type;
            // Пуля проходит через пустое и воду, но не через стены
            return type == CellType.Empty || type == CellType.Water;
        }

        // нанесение урона стене, true если стена разрушена
        public bool DamageWall(int row, int col)
        {
            if (_cells[row, col].Type != CellType.Wall) return false;

            _cells[row, col].Hp--;

            if (_cells[row, col].Hp <= 0)
            {
                // Стена разрушена
                _cells[row, col] = new Cell { Type = CellType.Empty, DisplayChar = ' ' };
                return true;
            }

            // вид стены от её хп 3=# 2=% 1=.
            _cells[row, col].DisplayChar = _cells[row, col].Hp switch
            {
                2 => '%',
                1 => '.',
                _ => '#'
            };

            return false;
        }

        public Cell GetCell(int row, int col) => _cells[row, col];
    }
}