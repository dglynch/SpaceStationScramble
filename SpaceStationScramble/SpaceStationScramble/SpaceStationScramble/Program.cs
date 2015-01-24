using System;

namespace SpaceStationScramble {
#if WINDOWS || XBOX
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args) {
            using (SpaceStationScrambleGame game = new SpaceStationScrambleGame()) {
                game.Run();
            }
        }
    }
#endif
}
