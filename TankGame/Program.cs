using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            System.Console.SetWindowSize(80, 22);
            System.Console.SetBufferSize(80, 22);
            Game game = new Game();
            game.Run();
        }
    }
}
