using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStationScramble {
    class EventGenerator {

        private Synchronizer syncronizer;
        //Slots mapped to time that slot will be free
        private Dictionary<EventSlot, long> eventSlots;
        private int minEventInterval;
        private int maxEventInterval;
        private int minDuration;
        private int maxDuration;

        public EventGenerator(Synchronizer syncronizer) {
            this.syncronizer = syncronizer;
            //Eventually these times will change as time goes on
            minEventInterval = 2000;
            maxEventInterval = 7000;
            minDuration = 5000;
            maxDuration = 7000;

            eventSlots = new Dictionary<EventSlot, long>();
            foreach(EventSlot slot in Enum.GetValues(typeof (EventSlot))) {
                eventSlots.Add(slot, 0);
            }
        }

        public DisasterEvent NextEvent() {
            //Next event time is last event time plus random value
            var slots = Enum.GetValues(typeof(EventSlot));
            EventSlot nextSlot = (EventSlot)(slots.GetValue(syncronizer.Next(0, slots.Length)));

            long nextEventTime = eventSlots[nextSlot] + syncronizer.Next(minEventInterval, maxEventInterval);
            long eventDuration = syncronizer.Next(minDuration, maxDuration); //TODO random durations?
            eventSlots[nextSlot] = nextEventTime + eventDuration;

            //Currently only creating gas leaks
            var steamColors = Enum.GetValues(typeof(SteamColor));
            SteamColor steamColor = (SteamColor)(steamColors.GetValue(syncronizer.Next(0, steamColors.Length)));
            return new GasLeakDisaster(nextSlot, nextEventTime, nextEventTime + eventDuration, steamColor);
        }
    }
}
