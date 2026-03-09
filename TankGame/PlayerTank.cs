using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class PlayerTank : Tank
    {
        //количество жизней игрока
        public int Lives { get; private set; }

        // Стартовая позиция нужна для возрождения
        private int _startRow;
        private int _startCol;

        // флаг неуязвимости после возрождения
        public bool IsInvincible { get; private set; }
        private int _invincibleTimer = 0;
        private const int InvincibleDuration = 40; // 40 тиков неуязвимости

        // moveCooldownMax = 3 для того чтобы игрок двигался быстрее врагов
        public PlayerTank(int row, int col) : base(row, col, Direction.Up, moveCooldownMax: 1)
        {
            Lives = 3;          // старт с 3 жизнями
            _startRow = row;
            _startCol = col;
        }

        // Пули игрока помечены как player
        protected override bool IsPlayerBullet() => true;

        // получение урона игроком, не умираем сразу, а теряем жизнь
        public void TakePlayerDamage()
        {
            // Если включена неуязвимость то урон не проходит
            if (IsInvincible) return;

            Lives--;

            if (Lives <= 0)
            {
                // Жизни кончились = смерть
                IsAlive = false;
            }
            else
            {
                Respawn();
            }
        }

        // возрождение на стартовой позиции
        private void Respawn()
        {
            Row = _startRow;
            Col = _startCol;
            Dir = Direction.Up;

            // неуязвимость
            IsInvincible = true;
            _invincibleTimer = InvincibleDuration;
        }

        // обновление таймера неуязвимости
        public void UpdateInvincibility()
        {
            if (!IsInvincible) return;

            _invincibleTimer--;
            if (_invincibleTimer <= 0)
            {
                IsInvincible = false;
            }
        }

        public void SetLives(int lives)
        {
            Lives = lives;
        }

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
