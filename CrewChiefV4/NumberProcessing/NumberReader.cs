﻿using System;
using System.Collections.Generic;
namespace CrewChiefV4
{
    public abstract class NumberReader
    {
        /**
         * Language specific implementation to speak an integer, using whatever rules and words this language requires.
         * Note this char array may contain only '0'.
         */
        protected abstract List<String> ConvertIntegerToSounds(char[] digits);

        /**
         * Language specific implementation to speak a number of hours, using whatever rules and words this language requires.
         * This might need to take the numbers of minutes, seconds and tenths into consideration.
         */
        protected abstract List<String> GetHoursSounds(int hours, int minutes, int seconds, int tenths);

        /**
         * Language specific implementation to speak a number of minutes, using whatever rules and words this language requires.
         * This might need to take the numbers of hours, seconds and tenths into consideration.
         */
        protected abstract List<String> GetMinutesSounds(int hours, int minutes, int seconds, int tenths);

        /**
         * Language specific implementation to speak a number of seconds, using whatever rules and words this language requires.
         * This might need to take the numbers of hours, minutes and tenths into consideration.
         */
        protected abstract List<String> GetSecondsSounds(int hours, int minutes, int seconds, int tenths);

        /**
         * Language specific implementation to speak a number of tenths, using whatever rules and words this language requires.
         * This might need to take the numbers of hours, minutes and seconds into consideration.
         */
        protected abstract List<String> GetTenthsSounds(int hours, int minutes, int seconds, int tenths);


        /**
         * Convert a timeSpan to some sound files, using the current language's implementation.
         */
        public List<String> ConvertTimeToSounds(TimeSpan timeSpan)
        {
            List<String> messageFolders = new List<String>();
            if (timeSpan != null)
            {
                // if the milliseconds in this timeSpan is > 949, when we turn this into tenths it'll get rounded up to 
                // ten tenths, which we can't have. So move the timespan on so this rounding doesn't happen
                if (timeSpan.Milliseconds > 949)
                {
                    timeSpan = timeSpan.Add(TimeSpan.FromMilliseconds(1000 - timeSpan.Milliseconds));
                }
                int tenths = (int)Math.Round((float)timeSpan.Milliseconds / 100f);

                // now call the language-specific implementations
                messageFolders.AddRange(GetHoursSounds(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, tenths));
                messageFolders.AddRange(GetMinutesSounds(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, tenths));
                messageFolders.AddRange(GetSecondsSounds(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, tenths));
                messageFolders.AddRange(GetTenthsSounds(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, tenths));
            }
            return messageFolders;
        }

        /**
         * Convert an integer to some sound files, using the current language's implementation.
         */
        public List<String> ConvertIntegerToSounds(int integer)
        {
            if (integer >= 0 && integer <= 99999)
            {
                return ConvertIntegerToSounds(integer.ToString().ToCharArray());
            }
            else
            {
                Console.WriteLine("Cannot convert integer " + integer + " valid range is 0 - 99999");
                return new List<String>();
            }
        }
    }
}
