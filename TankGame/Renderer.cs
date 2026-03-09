using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class Renderer
    {
        // Смещение карты от верхнего края консоли
        private const int MapOffsetRow = 2;
        private const int MapOffsetCol = 0;

        // DrawAll метод рисует всё за один проход
        public void DrawAll(Map map, PlayerTank player, List<EnemyTank> enemies, List<Bullet> bullets, int level, int score)
        {
            // рисуем карту
            DrawMap(map);

            // рисуем объекты поверх карты
            foreach (Bullet b in bullets)
            {
                if (b.IsAlive) DrawAt(b.Row, b.Col, '*', ConsoleColor.Yellow);
            }

            // рисуем врагов
            foreach (EnemyTank enemy in enemies)
            {
                if (enemy.IsAlive)
                    DrawAt(enemy.Row, enemy.Col, enemy.DisplayChar, ConsoleColor.Red);
            }

            // рисуем игрока
            if (player.IsAlive)
                DrawAt(player.Row, player.Col, player.DisplayChar, ConsoleColor.Green);

            // HUD
            DrawHUD(level, score, enemies);
        }

        // поклеточная отрисровка карты
        private void DrawMap(Map map)
        {
            for (int row = 0; row < map.Height; row++)
            {
                Console.SetCursorPosition(MapOffsetCol, MapOffsetRow + row);
                for (int col = 0; col < map.Width; col++)
                {
                    Cell cell = map.GetCell(row, col);
                    switch (cell.Type)
                    {
                        case CellType.Wall:
                            // Цвет стен зависит от хп
                            Console.ForegroundColor = cell.Hp == 3 ? ConsoleColor.White
                                                    : cell.Hp == 2 ? ConsoleColor.Gray
                                                    : ConsoleColor.DarkGray;
                            Console.Write(cell.DisplayChar);
                            break;
                        case CellType.Water:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write('~');
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write(' ');
                            break;
                    }
                }
            }
            Console.ResetColor();
        }

        private void DrawAt(int row, int col, char ch, ConsoleColor color)
        {
            Console.SetCursorPosition(MapOffsetCol + col, MapOffsetRow + row);
            Console.ForegroundColor = color;
            Console.Write(ch);
            Console.ResetColor();
        }

        // HUD
        private void DrawHUD(int level, int score, List<EnemyTank> enemies)
        {
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.White;

            // Счёт живых врагов
            int aliveEnemies = 0;
            foreach (EnemyTank e in enemies)
            {
                if (e.IsAlive) aliveEnemies++;
            }

            Console.Write($"  Уровень: {level}   Счёт: {score}   Враги: {aliveEnemies}   Управление: Стрелочки=Движение  Пробел=Выстрел  Q=Выход  ");
            Console.ResetColor();
        }

        // Метод отображения сообщения по центру экрана
        public void DrawMessage(string message, ConsoleColor color)
        {
            int row = Console.WindowHeight / 2;
            int col = Math.Max(0, (Console.WindowWidth - message.Length) / 2);
            Console.SetCursorPosition(col, row);
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }

        public void Clear()
        {
            Console.Clear();
        }
        
        public void HideCursor()
        {
            Console.CursorVisible = false;
        }
    }
}
