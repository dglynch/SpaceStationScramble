using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStationScramble {
    class EventGenerator {

        private Synchronizer synchronizer;
        //Slots mapped to time that slot will be free
        private Dictionary<EventSlot, long> p1EventSlots;
        private Dictionary<EventSlot, long> p2EventSlots;
        private int minEventInterval;
        private int maxEventInterval;
        private int minDuration;
        private int maxDuration;

        private long lastP1EventStarted;
        private long lastP2EventStarted;

        private int lastPlayer;

        public EventGenerator(Synchronizer synchronizer) {
            this.synchronizer = synchronizer;
            //Eventually these times will change as time goes on
            minEventInterval = 6000;
            maxEventInterval = 10000;
            minDuration = 10000;
            maxDuration = 14000;
            lastP1EventStarted = 0;
            lastP2EventStarted = 0;
            lastPlayer = 0;

            p1EventSlots = new Dictionary<EventSlot, long>();
            p2EventSlots = new Dictionary<EventSlot, long>();
            foreach (EventSlot slot in Enum.GetValues(typeof(EventSlot))) {
                p1EventSlots.Add(slot, 0);
                p2EventSlots.Add(slot, 0);
            }
        }

        public DisasterEvent NextEvent() {
            //Next event time is last event time plus random value
            if (lastPlayer == 1) {
                lastPlayer = 0;
                long nextEventTime;
                if (lastP1EventStarted == 0) {
                    nextEventTime = synchronizer.Next(3000, 5000);
                } else {
                    nextEventTime = lastP1EventStarted + synchronizer.Next(minEventInterval, maxEventInterval);
                }

                var slots = Enum.GetValues(typeof(EventSlot)).Cast<EventSlot>().ToList();
                List<EventSlot> goodEvents = new List<EventSlot>();
                foreach (var slot in slots) {
                    if (p1EventSlots[slot] < nextEventTime) {
                        goodEvents.Add(slot);
                    }
                }

                EventSlot nextSlot;
                if (goodEvents.Count > 0) {
                    nextSlot = goodEvents[synchronizer.Next(0, goodEvents.Count)];
                } else {
                    nextEventTime += 10000;
                    foreach (var slot in slots) {
                        if (p1EventSlots[slot] < nextEventTime) {
                            goodEvents.Add(slot);
                        }
                    }
                    nextSlot = goodEvents[synchronizer.Next(0, slots.Count)];
                }

                long eventDuration = synchronizer.Next(minDuration, maxDuration);
                p1EventSlots[nextSlot] = nextEventTime + eventDuration;
                lastP1EventStarted = nextEventTime;

                //Currently only creating gas leaks
                var steamColors = Enum.GetValues(typeof(SteamColor));
                SteamColor steamColor = (SteamColor)(steamColors.GetValue(synchronizer.Next(0, steamColors.Length)));
                return new GasLeakDisaster(nextSlot, nextEventTime, nextEventTime + eventDuration, steamColor);
            } else {
                //Generate player 2 event
                lastPlayer = 1;
                long nextEventTime;
                if (lastP2EventStarted == 0) {
                    nextEventTime = synchronizer.Next(3000, 5000);
                } else {
                    nextEventTime = lastP2EventStarted + synchronizer.Next(minEventInterval, maxEventInterval);
                }

                var slots = Enum.GetValues(typeof(EventSlot)).Cast<EventSlot>().ToList();
                List<EventSlot> goodEvents = new List<EventSlot>();
                foreach (var slot in slots) {
                    if (p2EventSlots[slot] < nextEventTime) {
                        goodEvents.Add(slot);
                    }
                }

                EventSlot nextSlot;
                if (goodEvents.Count > 0) {
                    nextSlot = goodEvents[synchronizer.Next(0, goodEvents.Count)];
                } else {
                    nextEventTime += 10000;
                    foreach (var slot in slots) {
                        if (p2EventSlots[slot] < nextEventTime) {
                            goodEvents.Add(slot);
                        }
                    }
                    nextSlot = goodEvents[synchronizer.Next(0, slots.Count)];
                }

                long eventDuration = synchronizer.Next(minDuration, maxDuration);
                p2EventSlots[nextSlot] = nextEventTime + eventDuration;
                lastP2EventStarted = nextEventTime;

                //Choose damaged item
                var stationParts = Enum.GetValues(typeof(StationPart));
                StationPart stationPart = (StationPart)(stationParts.GetValue(synchronizer.Next(0, stationParts.Length)));
                return new RepairDisaster(nextSlot, nextEventTime, nextEventTime + eventDuration, stationPart);
            }
        }
    }
}
