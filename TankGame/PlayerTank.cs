using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class PlayerTank : Tank
    {
        // moveCooldownMax = 3 для того чтобы игрок двигался быстрее врагов
        public PlayerTank(int row, int col) : base(row, col, Direction.Up, moveCooldownMax: 3) { }

        // Пули игрока помечены как player
        protected override bool IsPlayerBullet() => true;

        // Метод HandleInput принимает нажатую клавишу
        public Bullet? HandleInput(ConsoleKey key, Map map, System.Collections.Generic.List<Tank> allTanks)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    TryMove(Direction.Up, map, allTanks);
                    break;
                case ConsoleKey.DownArrow:
                    TryMove(Direction.Down, map, allTanks);
                    break;
                case ConsoleKey.LeftArrow:
                    TryMove(Direction.Left, map, allTanks);
                    break;
                case ConsoleKey.RightArrow:
                    TryMove(Direction.Right, map, allTanks);
                    break;
                case ConsoleKey.Spacebar:
                case ConsoleKey.Enter:
                    return Shoot();
            }
            return null;
        }
    }
}
