using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Шаблон для PlayerTank и EnemyTank

namespace TankGame
{
    public abstract class Tank
    {
        // Позиция танка на карте
        public int Row { get; protected set; }
        public int Col { get; protected set; }

        // Направление танка
        public Direction Dir { get; protected set; }

        // Жив ли танк
        public bool IsAlive { get; protected set; }

        // Символ танка по напрвлениям
        public char DisplayChar => Dir switch
        {
            Direction.Up => '^',
            Direction.Down => 'v',
            Direction.Left => '<',
            Direction.Right => '>',
            _ => '?'
        };

        // КД выстрела
        protected int _shootCooldown = 0;
        protected readonly int ShootCooldownMax; // 5 тиков между выстрелами

        // КД движения
        protected int _moveCooldown = 0;
        protected int _moveCooldownMax;

        protected Tank(int row, int col, Direction dir, int moveCooldownMax, int shootCooldownMax = 5)
        {
            Row = row;
            Col = col;
            Dir = dir;
            IsAlive = true;
            _moveCooldownMax = moveCooldownMax;
            ShootCooldownMax = shootCooldownMax;
        }

        // Метод TryMove пытается сдвинуться в направлении dir
        // Принимает Map для проверки коллизий
        public bool TryMove(Direction dir, Map map, List<Tank> allTanks)
        {
            if (_moveCooldown > 0)
            {
                _moveCooldown--;
                return false;
            }

            // Направление, разворот без движения тоже считается
            Direction oldDir = Dir;
            Dir = dir;

            // Новая позиция
            int newRow = Row, newCol = Col;
            switch (dir)
            {
                case Direction.Up: newRow--; break;
                case Direction.Down: newRow++; break;
                case Direction.Left: newCol--; break;
                case Direction.Right: newCol++; break;
            }

            // Проверка проходимости клетки карты
            if (!map.IsPassable(newRow, newCol)) return false;

            // Проверка столкновения
            foreach (Tank other in allTanks)
            {
                if (other == this || !other.IsAlive) continue;
                if (other.Row == newRow && other.Col == newCol) return false;
            }

            // Двигаем танк
            Row = newRow;
            Col = newCol;
            _moveCooldown = _moveCooldownMax;
            return true;
        }

        // Метод Shoot создаёт пулю перед танком.
        public Bullet? Shoot()
        {
            if (_shootCooldown > 0) return null;

            // Пуля стартует из клетки перед танком
            int bulletRow = Row, bulletCol = Col;
            switch (Dir)
            {
                case Direction.Up: bulletRow--; break;
                case Direction.Down: bulletRow++; break;
                case Direction.Left: bulletCol--; break;
                case Direction.Right: bulletCol++; break;
            }

            _shootCooldown = ShootCooldownMax;
            return new Bullet(bulletRow, bulletCol, Dir, IsPlayerBullet());
        }

        // Определяет тип пули, вражеская или игрока
        protected abstract bool IsPlayerBullet();

        // Получение урона
        public void TakeDamage()
        {
            IsAlive = false;
        }

        // Обновление КД
        public void UpdateCooldowns()
        {
            if (_shootCooldown > 0) _shootCooldown--;
        }
    }
}
