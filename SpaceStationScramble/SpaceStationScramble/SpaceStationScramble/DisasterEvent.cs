using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStationScramble {
    abstract class DisasterEvent {

        public EventSlot Slot {
            get;
            protected set;
        }

        public PlayerNumber VisibleToPlayer {
            get;
            protected set;
        }

        public long StartTime {
            get;
            protected set;
        }

        public long EndTime {
            get;
            protected set;
        }

        public Vector2 Position {
            get;
            protected set;
        }

        public DisasterEvent(EventSlot slot, long startTime, long endTime) {
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Slot = slot;
        }

        public void Update() {

        }

        public abstract void Draw(SpriteBatch spriteBatch);
    }

    public enum EventSlot {
        North,
        South,
        East,
        West,
        Center
    };
}
