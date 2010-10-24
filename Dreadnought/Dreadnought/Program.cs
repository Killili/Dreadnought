using System;

namespace Dreadnought
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
		  [STAThread]
        static void Main(string[] args)
        {
            using (Dreadnought.Game game = new Dreadnought.Game())
            {
                game.Run();
            }
        }
    }
#endif
}

