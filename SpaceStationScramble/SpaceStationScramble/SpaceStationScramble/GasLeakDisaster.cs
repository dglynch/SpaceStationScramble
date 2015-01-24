using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStationScramble {
    class GasLeakDisaster : DisasterEvent {
        private readonly Vector2 northDrawLoc = new Vector2(640, 60);
        private readonly Vector2 southDrawLoc = new Vector2(640, 660);
        private readonly Vector2 eastDrawLoc = new Vector2(980, 360);
        private readonly Vector2 westDrawLoc = new Vector2(300, 360);

        private SteamColor steamColor;

        public GasLeakDisaster(EventSlot slot, long startTime, long endTime) :
            base(startTime, endTime) {
            
            //This is stuff set on a class by class basis
            VisibleToPlayer = PlayerNumber.TWO;

            //TODO: Random steam color set by generator so its synced
            steamColor = SteamColor.Blue;

            switch (slot) {
                case EventSlot.North:
                    Position = northDrawLoc;
                    break;
                case EventSlot.South:
                    Position = southDrawLoc;
                    break;
                case EventSlot.East:
                    Position = eastDrawLoc;
                    break;
                case EventSlot.West:
                    Position = westDrawLoc;
                    break;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            switch (steamColor) {
                case SteamColor.Red:
                    spriteBatch.Draw(SpaceStationScrambleGame.steamTexture, Position, Color.Red);
                    break;
                case SteamColor.Blue:
                    spriteBatch.Draw(SpaceStationScrambleGame.steamTexture, Position, Color.Blue);
                    break;
                case SteamColor.Green:
                    spriteBatch.Draw(SpaceStationScrambleGame.steamTexture, Position, Color.Green);
                    break;
            }
        }
    }
    public enum SteamColor {
        Red,
        Green,
        Blue
    };

    public enum EventSlot {
        North,
        South,
        East,
        West
    };
}
