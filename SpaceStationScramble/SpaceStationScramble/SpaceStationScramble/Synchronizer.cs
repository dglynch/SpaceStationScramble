﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpaceStationScramble {

    class Synchronizer {

        private const int KeyLength = 3;
        private const int CheckLength = 1;

        private Random random;

        public string GenerateKeyCode() {
            random = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < KeyLength; i++) {
                int wordIndex = random.Next(KeyList.Count);
                sb.Append(KeyList[wordIndex]);
            }
            random = new Random(sb.ToString().GetHashCode());
            for (int i = 0; i < CheckLength; i++) {
                int wordIndex = random.Next(KeyList.Count);
                sb.Append(KeyList[wordIndex]);
            }
            return sb.ToString();
        }

        public void AcceptKeyCode(string keyCode) {
            string[] words = SplitCamelCase(keyCode);
            if (words.Length != KeyLength + CheckLength) {
                throw new InvalidKeyCodeException();
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < KeyLength; i++) {
                sb.Append(words[i]);
            }
            random = new Random(sb.ToString().GetHashCode());
            for (int i = 0; i < CheckLength; i++) {
                int wordIndex = random.Next(KeyList.Count);
                if (KeyList[wordIndex] != words[i + KeyLength]) {
                    throw new InvalidKeyCodeException();
                }
            }
        }

        public int Next() {
            return random.Next();
        }

        public int Next(int maxValue) {
            return random.Next(maxValue);
        }

        public int Next(int minValue, int maxValue) {
            return random.Next(minValue, maxValue);
        }

        public void NextBytes(byte[] buffer) {
            random.NextBytes(buffer);
        }

        public double NextDouble() {
            return random.NextDouble();
        }

        private static string[] SplitCamelCase(string input) {
            return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim().Split(null);
        }

        private readonly List<string> KeyList = new List<string>() {
            "Adjust",
            "Answer",
            "Audio",
            "Avoid",
            "Bauble",
            "Blank",
            "Brisk",
            "Bump",

        };

    }

    public class InvalidKeyCodeException : Exception {
    }

}
