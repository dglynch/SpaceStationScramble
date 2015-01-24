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

        public EventGenerator(Synchronizer syncronizer) {
            this.syncronizer = syncronizer;
            //Eventually these times will change as time goes on
            minEventInterval = 2000;
            maxEventInterval = 7000;

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
            eventSlots[nextSlot] = nextEventTime;

            long eventDuration = 3000; //TODO random durations?

            return new DisasterEvent(nextSlot, nextEventTime, nextEventTime + eventDuration);
        }
    }
}
