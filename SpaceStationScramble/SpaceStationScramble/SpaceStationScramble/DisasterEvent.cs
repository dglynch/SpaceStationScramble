using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStationScramble {
    class DisasterEvent {
        private EventSlot slot;
        private Vector2 position;
        private Texture2D texture;

        public PlayerNumber VisibleToPlayer {
            get;
            private set;
        }

        public long StartTime {
            get;
            private set;
        }

        public long EndTime {
            get;
            private set;
        }

        //Fields that will really belong to subclasses
        private SteamColor steamColor;

        public DisasterEvent(EventSlot nextSlot, long startTime, long endTime) {
            this.slot = nextSlot;
            this.StartTime = startTime;
            this.EndTime = endTime;

            //This is stuff set on a class by class basis
            VisibleToPlayer = PlayerNumber.TWO;

            //TODO: Random steam color set by generator so its synced
            steamColor = SteamColor.Blue;
        }

        public void Update() {

        }

        public void setPosition(Vector2 position) {
            this.position = position;
        }

        public void Draw(SpriteBatch spriteBatch) {
            switch (steamColor) {
                case SteamColor.Red:
                    spriteBatch.Draw(texture, position, Color.Red);
                    break;
                case SteamColor.Blue:
                    spriteBatch.Draw(texture, position, Color.Blue);
                    break;
                case SteamColor.Green:
                    spriteBatch.Draw(texture, position, Color.Green);
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
        NorthValves,
        SouthValves,
        EastValves,
        WestValves
    }
}
