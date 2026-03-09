// Game.cs — ПОЛНЫЙ ОБНОВЛЁННЫЙ ФАЙЛ

using System;
using System.Collections.Generic;
using System.Threading;

namespace TankGame
{
    public class Game
    {
        private Map _map;
        private PlayerTank _player;
        private List<EnemyTank> _enemies;
        private List<Bullet> _bullets;
        private Renderer _renderer;

        private int _level;
        private int _score;
        private bool _isRunning;

        // Задержка тика в миллисекундах
        private const int TickMs = 80;

        public Game()
        {
            _renderer = new Renderer();
            _level = 1;
            _score = 0;
            _bullets = new List<Bullet>();
            _enemies = new List<EnemyTank>();
        }

        // Run запускает игру
        public void Run()
        {
            _renderer.HideCursor();
            _isRunning = true;

            while (_isRunning)
            {
                StartLevel(_level);
                GameResult result = RunLevel();

                if (result == GameResult.Victory)
                {
                    _renderer.DrawAll(_map, _player, _enemies, _bullets, _level, _score);
                    _renderer.DrawMessage($"  Уровень {_level} пройден!", ConsoleColor.Green);
                    Thread.Sleep(2000); // 2 секунды перед следующим уровнем
                    _level++;
                }
                else if (result == GameResult.Defeat)
                {
                    _renderer.DrawAll(_map, _player, _enemies, _bullets, _level, _score);
                    _renderer.DrawMessage("  Проигрыш. Нажмите любую кнопку  ", ConsoleColor.Red);
                    Console.ReadKey(true);
                    _isRunning = false;
                }
                else
                {
                    _isRunning = false;
                }
            }

            _renderer.Clear();
            Console.CursorVisible = true;
            Console.WriteLine($"Спасибо за игру! Счёт: {_score}");
        }

        private enum GameResult { Victory, Defeat, Quit }

        // Инициализация уровня
        private void StartLevel(int level)
        {
            _bullets.Clear();
            _enemies.Clear();

            string[] mapLines = MapData.GetMap(level);
            _map = new Map(mapLines);

            // Сканируем карту в поисках 'P' и 'E' чтобы расставить игрока и врагоы
            for (int row = 0; row < mapLines.Length; row++)
            {
                for (int col = 0; col < mapLines[row].Length; col++)
                {
                    char ch = mapLines[row][col];
                    if (ch == 'P')
                    {
                        // если игрок уже существует то сохраняем его жизни. Если первый уровень — создаём нового.
                        if (_player == null)
                        {
                            _player = new PlayerTank(row, col);
                        }
                        else
                        {
                            // Перемещаем игрока на стартовую позицию нового уровня и сохраняем количество жизней
                            int savedLives = _player.Lives;
                            _player = new PlayerTank(row, col);
                            _player.SetLives(savedLives);
                        }
                    }
                    else if (ch == 'E')
                    {
                        _enemies.Add(new EnemyTank(row, col));
                    }
                }
            }

            if (_player == null)
                _player = new PlayerTank(1, 1);

            // Доббавление доп врагов по мере возростания уровня
            int extraEnemies = Math.Min(level - 1, 4);
            AddExtraEnemies(extraEnemies);

            _renderer.Clear();
        }

        // Доп враги на случайные клетки
        private void AddExtraEnemies(int count)
        {
            Random rng = new Random();
            int attempts = 0;
            int added = 0;

            while (added < count && attempts < 1000)
            {
                attempts++;
                int row = rng.Next(1, _map.Height - 1);
                int col = rng.Next(1, _map.Width - 1);

                // Проверка на свободу клетки и удаленность от игррока
                if (!_map.IsPassable(row, col)) continue;
                if (Math.Abs(row - _player.Row) + Math.Abs(col - _player.Col) < 5) continue;

                bool occupied = false;
                foreach (EnemyTank e in _enemies)
                {
                    if (e.Row == row && e.Col == col) { occupied = true; break; }
                }
                if (occupied) continue;

                _enemies.Add(new EnemyTank(row, col));
                added++;
            }
        }

        // Основной цикл
        private GameResult RunLevel()
        {
            List<Tank> allTanks = new List<Tank>();
            allTanks.Add(_player);
            allTanks.AddRange(_enemies);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                    if (keyInfo.Key == ConsoleKey.Q)
                        return GameResult.Quit;

                    Bullet? playerBullet = _player.HandleInput(keyInfo.Key, _map, allTanks);
                    if (playerBullet != null) _bullets.Add(playerBullet);
                }

                _player.UpdateCooldowns();

                // обновляем таймер неуязвимости каждый тик
                _player.UpdateInvincibility();

                foreach (EnemyTank enemy in _enemies)
                {
                    Bullet? enemyBullet = enemy.Update(_player, _map, allTanks);
                    if (enemyBullet != null) _bullets.Add(enemyBullet);
                }

                UpdateBullets(allTanks);

                if (!_player.IsAlive) return GameResult.Defeat;

                bool anyEnemyAlive = false;
                foreach (EnemyTank e in _enemies)
                {
                    if (e.IsAlive) { anyEnemyAlive = true; break; }
                }
                if (!anyEnemyAlive) return GameResult.Victory;

                _renderer.DrawAll(_map, _player, _enemies, _bullets, _level, _score);

                Thread.Sleep(TickMs);
            }
        }

        private void UpdateBullets(List<Tank> allTanks)
        {
            for (int i = 0; i < _bullets.Count; i++)
            {
                Bullet b = _bullets[i];
                if (!b.IsAlive) continue;

                b.Move();

                if (b.Row < 0 || b.Row >= _map.Height || b.Col < 0 || b.Col >= _map.Width)
                {
                    b.IsAlive = false;
                    continue;
                }

                if (!_map.IsBulletPassable(b.Row, b.Col))
                {
                    _map.DamageWall(b.Row, b.Col);
                    b.IsAlive = false;
                    continue;
                }

                foreach (Tank tank in allTanks)
                {
                    if (!tank.IsAlive) continue;
                    if (tank.Row != b.Row || tank.Col != b.Col) continue;

                    bool hitPlayer = b.IsPlayerBullet == false && tank is PlayerTank;
                    bool hitEnemy = b.IsPlayerBullet == true && tank is EnemyTank;

                    if (hitPlayer)
                    {
                        ((PlayerTank)tank).TakePlayerDamage();
                        b.IsAlive = false;
                        break;
                    }
                    if (hitEnemy)
                    {
                        tank.TakeDamage();
                        b.IsAlive = false;
                        _score += 100; // за каждого убитого врага +100 очков
                        break;
                    }
                }
            }

            _bullets.RemoveAll(b => !b.IsAlive);
        }
    }
}
