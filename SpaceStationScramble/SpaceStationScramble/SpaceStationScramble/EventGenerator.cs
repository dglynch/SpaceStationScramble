using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStationScramble {
    class EventGenerator {

        private Synchronizer synchronizer;
        //Slots mapped to time that slot will be free
        private Dictionary<EventSlot, long> eventSlots;
        private int minEventInterval;
        private int maxEventInterval;
        private int minDuration;
        private int maxDuration;
        private long lastEventStarted;

        public EventGenerator(Synchronizer synchronizer) {
            this.synchronizer = synchronizer;
            //Eventually these times will change as time goes on
            minEventInterval = 6000;
            maxEventInterval = 10000;
            minDuration = 10000;
            maxDuration = 14000;
            lastEventStarted = 0;

            eventSlots = new Dictionary<EventSlot, long>();
            foreach(EventSlot slot in Enum.GetValues(typeof (EventSlot))) {
                eventSlots.Add(slot, 0);
            }
        }

        public DisasterEvent NextEvent() {
            //Next event time is last event time plus random value
            long nextEventTime;
            if (lastEventStarted == 0) {
                nextEventTime = synchronizer.Next(3000, 5000);
            } else {
                nextEventTime = lastEventStarted + synchronizer.Next(minEventInterval, maxEventInterval);
            }

            var slots = Enum.GetValues(typeof(EventSlot)).Cast<EventSlot>().ToList();
            List<EventSlot> goodEvents = new List<EventSlot>();
            foreach (var slot in slots) {
                if (eventSlots[slot] < nextEventTime) {
                    goodEvents.Add(slot);
                }
            }

            EventSlot nextSlot;
            if (goodEvents.Count > 0) {
                nextSlot = goodEvents[synchronizer.Next(0, goodEvents.Count)];
            } else {
                nextEventTime += 10000;
                foreach (var slot in slots) {
                    if (eventSlots[slot] < nextEventTime) {
                        goodEvents.Add(slot);
                    }
                }
                nextSlot = goodEvents[synchronizer.Next(0, slots.Count)];
            }

            long eventDuration = synchronizer.Next(minDuration, maxDuration);
            eventSlots[nextSlot] = nextEventTime + eventDuration;
            lastEventStarted = nextEventTime;

            //Currently only creating gas leaks
            var steamColors = Enum.GetValues(typeof(SteamColor));
            SteamColor steamColor = (SteamColor)(steamColors.GetValue(synchronizer.Next(0, steamColors.Length)));
            return new GasLeakDisaster(nextSlot, nextEventTime, nextEventTime + eventDuration, steamColor);
        }
    }
}
