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
        private readonly Vector2 centerDrawLoc = new Vector2(640, 360);

        public SteamColor SteamColor {
            get;
            private set;
        }

        public GasLeakDisaster(EventSlot slot, long startTime, long endTime, SteamColor steamColor) :
            base(slot, startTime, endTime) {

            VisibleToPlayer = PlayerNumber.TWO;
            SteamColor = steamColor;

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
                case EventSlot.Center:
                    Position = centerDrawLoc;
                    break;
            }
        }
        public override void Draw(SpriteBatch spriteBatch) {
            switch (SteamColor) {
                case SteamColor.Red:
                    spriteBatch.Draw(SpaceStationScrambleGame.steamTexture, Position, Color.Red);
                    break;
                case SteamColor.Blue:
                    spriteBatch.Draw(SpaceStationScrambleGame.steamTexture, Position, Color.Blue);
                    break;
                case SteamColor.Green:
                    spriteBatch.Draw(SpaceStationScrambleGame.steamTexture, Position, Color.Green);
                    break;
                case SteamColor.Yellow:
                    spriteBatch.Draw(SpaceStationScrambleGame.steamTexture, Position, Color.Yellow);
                    break;
            }
        }

        public override string ToString() {
            return "Gas Leak - " + Slot + " - " + SteamColor;
        }
    }
    public enum SteamColor {
        Red,
        Green,
        Blue,
        Yellow
    };
}
