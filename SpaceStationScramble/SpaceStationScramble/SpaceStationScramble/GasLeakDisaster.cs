using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStationScramble {
    class GasLeakDisaster : DisasterEvent {
        private readonly Vector2 northDrawLoc = new Vector2(640, 280);
        private readonly Vector2 southDrawLoc = new Vector2(640, 570);
        private readonly Vector2 eastDrawLoc = new Vector2(800, 420);
        private readonly Vector2 westDrawLoc = new Vector2(475, 420);
        private readonly Vector2 centerDrawLoc = new Vector2(640, 420);

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
            Vector2 textureOffset = new Vector2(-SpaceStationScrambleGame.steamTexture.Width / 2,
                -SpaceStationScrambleGame.steamTexture.Height / 2);
            switch (SteamColor) {
                case SteamColor.Red:
                    spriteBatch.Draw(SpaceStationScrambleGame.steamTexture, Position + textureOffset, Color.Red);
                    break;
                case SteamColor.Blue:
                    spriteBatch.Draw(SpaceStationScrambleGame.steamTexture, Position + textureOffset, Color.Blue);
                    break;
                case SteamColor.Green:
                    spriteBatch.Draw(SpaceStationScrambleGame.steamTexture, Position + textureOffset, Color.DarkGreen);
                    break;
                case SteamColor.Yellow:
                    spriteBatch.Draw(SpaceStationScrambleGame.steamTexture, Position + textureOffset, Color.Yellow);
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
