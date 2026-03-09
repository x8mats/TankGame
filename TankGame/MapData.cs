using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public static class MapData
    {
        //   '#' — обычная стена (разрушаемая)
        //   '~' — вода (пули проходят, танки нет)
        //   ' ' — пустое место
        //   'P' — стартовая позиция игрока
        //   'E' — стартовая позиция врага (будет заменена при загрузке)

        // Карта 1
        private static readonly string[] _map1 = new string[]
        {
            "####################",
            "#P      #    ~     #",
            "#  ###  #  ~~~~~   #",
            "#  #    #    ~     #",
            "#  ###     #####   #",
            "#      ##          #",
            "#  E   ##   E      #",
            "#      ##          #",
            "#  #####   ######  #",
            "#    ~     #    #  #",
            "#  ~~~~~   #  E #  #",
            "#    ~          #  #",
            "#       ######     #",
            "#                  #",
            "####################",
        };

        // Карта 2
        private static readonly string[] _map2 = new string[]
        {
            "####################",
            "#P  ##      ##  E  #",
            "#   ##  ~~  ##     #",
            "#          ###     #",
            "######      ##  ####",
            "#     ###          #",
            "#  E  ###   E      #",
            "#     ###          #",
            "####      ##########",
            "#      ~       E   #",
            "#     ~~~          #",
            "#      ~    ####   #",
            "#           ##     #",
            "#  E               #",
            "####################",
        };

        // Карта 3
        private static readonly string[] _map3 = new string[]
        {
            "####################",
            "#P #   #   #   #  E#",
            "# ## # # # # # ## #",
            "#    # # # # #    #",
            "# ## # # # # # ## #",
            "#  # #   #   # #  #",
            "## # ######### # ##",
            "#  #           #  #",
            "# ## ######### ## #",
            "#  E #   #   # E  #",
            "#    # # # # #    #",
            "# ## # # # # # ## #",
            "#    #   #   #    #",
            "#                  #",
            "####################",
        };

        public static string[] GetMap(int mapIndex)
        {
            string[] source = mapIndex switch
            {
                1 => _map1,
                2 => _map2,
                3 => _map3,
                // Если уровень больше 3 — циклично возвращаемся к картам
                _ => GetMap((mapIndex - 1) % 3 + 1)
            };

            return (string[])source.Clone();
        }

        // Количество доступных карт
        public static int MapCount => 3;
    }
}

