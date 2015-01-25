using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStationScramble {
    class RepairDisaster : DisasterEvent {

        private readonly Vector2 northDrawLoc = new Vector2(640, 120);
        private readonly Vector2 southDrawLoc = new Vector2(640, 600);
        private readonly Vector2 eastDrawLoc = new Vector2(920, 360);
        private readonly Vector2 westDrawLoc = new Vector2(360, 360);
        private readonly Vector2 centerDrawLoc = new Vector2(640, 360);

        public StationPart StationPart {
            get;
            private set;
        }

        public RepairDisaster(EventSlot slot, long startTime, long endTime, StationPart stationPart) :
            base(slot, startTime, endTime) {

            StationPart = stationPart;

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
            spriteBatch.Draw(SpaceStationScrambleGame.circleTexture, new Rectangle((int)Position.X - 64, (int)Position.Y - 64, 128, 128) , new Color(255, 2, 5, 125));
            spriteBatch.Draw(SpaceStationScrambleGame.alarmTexture, Position
                - new Vector2(SpaceStationScrambleGame.alarmTexture.Width / 2,
                    SpaceStationScrambleGame.alarmTexture.Height / 2), Color.White);
        }

        public override string ToString() {
            return "Part Repair - " + Slot + " - " + StationPart;
        }

    }

    public enum StationPart {
        SatelliteDish,
        O2Tank,
        Hatch,
        Pipe
    }
}
