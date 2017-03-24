using System;
using System.Diagnostics;
using System.Windows.Forms;
using WaveEngine.Adapter;

namespace Mad_Head_Puzzle
{
    static class Program
    {
        public static App game;
		[STAThread]
        static void Main()
        {
            using (game = new App())
            {
                game.Run();
            }
        }
    }
}

