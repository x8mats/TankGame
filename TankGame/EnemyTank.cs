using System;
using System.Collections.Generic;

namespace TankGame
{
    public class EnemyTank : Tank
    {
        private static readonly Random _rng = new Random();

        private int _dirChangeCooldown = 0;
        // Счётчик тиков до следующей смены направления
        private const int DirChangeCooldownMax = 60;
        // Максимальная дальность видимости врага в клетках
        private const int MaxSightRange = 8;


        private int _spotCooldown = 0;

        // Сколько тиков враг прицеливается перед первым выстрелом.
        private const int SpotCooldownMax = 10;

        // видел ли враг игрока в прошлом тике
        private bool _playerWasVisible = false;

        // moveCooldownMax = 6 чтобы враги двигались медленнее игрока
        // (Direction)_rng.Next(4) = случайное начальное направление
        // Next(4) = 0,1,2,3 джля Direction
        // shootCooldownMax кд выстрела
        public EnemyTank(int row, int col) : base(row, col, (Direction)_rng.Next(4), moveCooldownMax: 40, shootCooldownMax: 80)
        {
            _shootCooldown = ShootCooldownMax;
        }

        protected override bool IsPlayerBullet() => false;

        public Bullet? Update(PlayerTank player, Map map, List<Tank> allTanks)
        {
            if (!IsAlive) return null;

            UpdateCooldowns();

            // Проверка на игрока в поле зрения
            Bullet? shot = TryShootAtPlayer(player, map);
            if (shot != null) return shot;

            // Движение
            MoveAI(map, allTanks);

            return null;
        }

        // Проверяем прямую видимость игрока и стреляем
        private Bullet? TryShootAtPlayer(PlayerTank player, Map map)
        {
            if (!player.IsAlive) return null;

            Direction? targetDir = null;

            if (player.Col == Col)
                targetDir = player.Row < Row ? Direction.Up : Direction.Down;
            else if (player.Row == Row)
                targetDir = player.Col < Col ? Direction.Left : Direction.Right;

            // Игрок не на одной линии то сбрасываем состояние прицеливания
            if (targetDir == null)
            {
                // Если игрок ушёл с линии сбрасываем прицел.
                // Когда снова окажется на линии отсчёт начнётся заново.
                _playerWasVisible = false;
                _spotCooldown = 0;
                return null;
            }

            // проверка дальности
            int distance = Math.Abs(player.Row - Row) + Math.Abs(player.Col - Col);
            if (distance > MaxSightRange)
            {
                _playerWasVisible = false;
                _spotCooldown = 0;
                return null;
            }

            // нет ли стен на пути
            if (!IsLineOfSightClear(Row, Col, player.Row, player.Col, map))
            {
                // Стена перекрыла обзор — сбрасываем прицел
                _playerWasVisible = false;
                _spotCooldown = 0;
                return null;
            }

            Dir = targetDir.Value;

            if (!_playerWasVisible)
            {
                // Только что увидели игрока впервые 
                // Запускаем таймер прицеливания и выстрела нет
                _playerWasVisible = true;
                _spotCooldown = SpotCooldownMax;
                return null; // не стреляем, только прицеливаемся
            }

            // Игрок был виден и в прошлом тике.
            if (_spotCooldown > 0)
            {
                _spotCooldown--;
                return null; // не стреляем
            }

            return Shoot();
        }

        // Проверка прямой видимости между двумя точками, идём от (r1,c1) к (r2,c2) и проверяем IsBulletPassable
        private bool IsLineOfSightClear(int r1, int c1, int r2, int c2, Map map)
        {
            if (c1 == c2)
            {
                int step = r2 > r1 ? 1 : -1;
                for (int r = r1 + step; r != r2; r += step)
                    if (!map.IsBulletPassable(r, c1)) return false;
                return true;
            }
            if (r1 == r2)
            {
                int step = c2 > c1 ? 1 : -1;
                for (int c = c1 + step; c != c2; c += step)
                    if (!map.IsBulletPassable(r1, c)) return false;
                return true;
            }
            return false;
        }

        // Рандомное движение врага
        private void MoveAI(Map map, List<Tank> allTanks)
        {
            if (_dirChangeCooldown > 0)
            {
                _dirChangeCooldown--;
            }
            else
            {
                // Рандомное направление
                Dir = (Direction)_rng.Next(4);
                _dirChangeCooldown = DirChangeCooldownMax;
            }

            // Движжение в текущем направлении
            bool moved = TryMove(Dir, map, allTanks);

            // Смена направления при неудаче
            if (!moved)
            {
                Dir = (Direction)_rng.Next(4);
                _dirChangeCooldown = DirChangeCooldownMax / 2;
            }
        }
    }
}
