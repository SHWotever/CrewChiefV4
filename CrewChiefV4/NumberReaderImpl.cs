﻿using System;
using System.Collections.Generic;

namespace CrewChiefV4.NumberProcessing
{
    public class NumberReaderImpl : NumberReader
    {
        private static String folderNumbersStub = "numbers/";

        private static String folderPoint = "numbers/point";
        private static String folderThousand = "numbers/thousand";
        private static String folderThousandAnd = "numbers/thousand_and";
        private static String folderHundred = "numbers/hundred";
        private static String folderHundredAnd = "numbers/hundred_and";
        private static String folderZeroZero = "numbers/zerozero";
        private static String folderTenths = "numbers/tenths";
        private static String folderTenth = "numbers/tenth";
        private static String folderSeconds = "numbers/seconds";
        private static String folderSecond = "numbers/second";
        private static String folderMinutes = "numbers/minutes";
        private static String folderMinute = "numbers/minute"; 
        private static String folderHours = "numbers/hours";
        private static String folderHour = "numbers/hour";

        private Random random = new Random();

        protected override List<String> GetHoursSounds(int hours, int minutes, int seconds, int tenths)
        {
            List<String> messages = new List<String>();
            if (hours > 0)
            {
                messages.Add(folderNumbersStub + hours);
                if (hours == 1)
                {
                    messages.Add(folderHour);
                }
                else
                {
                    messages.Add(folderHours);
                }
            }
            Console.WriteLine(String.Join(", ", messages));
            return messages;
        }

        protected override List<String> GetMinutesSounds(int hours, int minutes, int seconds, int tenths)
        {
            List<String> messages = new List<String>();
            if (minutes > 0)
            {
                messages.Add(folderNumbersStub + minutes);
                // only read the "minutes" part if there are no seconds or tenths, or if we've read some hours
                if ((seconds == 0 && tenths == 0) || hours > 0)
                {
                    if (minutes == 1)
                    {
                        messages.Add(folderMinute);
                    }
                    else
                    {
                        messages.Add(folderMinutes);
                    }
                }
            }
            Console.WriteLine(String.Join(", ", messages));
            return messages;
        }

        protected override List<String> GetSecondsSounds(int hours, int minutes, int seconds, int tenths)
        {
            List<String> messages = new List<String>();
            // special case here - if we're reading a time which has hours, the seconds aren't significant so ignore them
            if (hours == 0)
            {
                // if we've read some minutes, the zeros in the seconds must be read.
                // if we're going to read some tenths as well and the number of seconds is 0, say "zero-zero".
                if (minutes > 0 && seconds == 0 && tenths > 0)
                {
                    messages.Add(folderZeroZero);
                }
                else if (seconds > 0)
                {
                    // again, if we've read some minutes, the zeros in the seconds must be read.
                    // There are some specific sounds for this - 01 to 09 which combine the "oh" sound with the number
                    if (minutes > 0 && seconds < 10)
                    {
                        messages.Add(folderNumbersStub + "0" + seconds);
                    }
                    else
                    {
                        messages.Add(folderNumbersStub + seconds);
                    }
                    // we only add "seconds" here if we've not read "minutes", and there are no tenths
                    // Note that we don't have to check the hours here because we don't read seconds if there are hours
                    if (minutes == 0 && tenths == 0)
                    {
                        if (seconds == 1)
                        {
                            messages.Add(folderSecond);
                        }
                        else
                        {
                            messages.Add(folderSeconds);
                        }
                    }
                }
            }
            Console.WriteLine(String.Join(", ", messages));
            return messages;
        }

        protected override List<String> GetTenthsSounds(int hours, int minutes, int seconds, int tenths)
        {
            List<String> messages = new List<String>();
            // special case here - if we're reading a time which has hours, the tenths aren't significant so ignore them. 
            // Still read the tenths if we have > 0 minutes, because this is common for laptimes
            if (hours == 0 && tenths < 10)
            {
                // if there are no minutes or seconds, just say "1 tenth" or "2 tenths" etc
                if (minutes == 0 && seconds == 0)
                {
                    messages.Add(folderNumbersStub + tenths);
                    if (tenths == 1)
                    {
                        messages.Add(folderTenth);
                    }
                    else
                    {
                        messages.Add(folderTenths);
                    }
                }
                else
                {
                    // there are some more compact number sounds for tenths which include point and seconds - e.g. "point 3 seconds".
                    // We can use them or not, it makes sense either way, so we can mix it up a little here and sometimes include 
                    // the "seconds", sometimes not
                    if (tenths > 0)
                    {
                        if (random.NextDouble() > 0.5)
                        {
                            messages.Add(folderPoint + tenths + "seconds");
                        }
                        else
                        {
                            messages.Add(folderPoint);
                            messages.Add(folderNumbersStub + tenths);
                        }
                    }
                }
            }
            Console.WriteLine(String.Join(", ", messages));
            return messages;
        }

        // only works with 5 or less digits
        protected override List<String> ConvertIntegerToSounds(char[] digits)
        {
            List<String> messages = new List<String>();
            if (digits.Length == 0 || (digits.Length == 1 && digits[0] == '0'))
            {
                messages.Add(folderNumbersStub + 0);
            }
            String tensAndUnits = null;
            String hundreds = null;
            String thousands = null;

            if (digits.Length == 1 || (digits[digits.Length - 2] == '0' && digits[digits.Length - 1] != '0'))
            {
                tensAndUnits = digits[digits.Length - 1].ToString();
            }
            else if (digits[digits.Length - 2] != '0' || digits[digits.Length - 1] != '0')
            {
                tensAndUnits = digits[digits.Length - 2].ToString() + digits[digits.Length - 1].ToString();
            }
            if (digits.Length == 4 && digits[0] == '1' && digits[1] != '0')
            {
                hundreds = digits[0].ToString() + digits[1].ToString();
            }
            else
            {
                if (digits.Length >= 3)
                {
                    if (digits[digits.Length - 3] != '0')
                    {
                        hundreds = digits[digits.Length - 3].ToString();
                    }
                    if (digits.Length == 4)
                    {
                        thousands = digits[0].ToString();
                    }
                    else if (digits.Length == 5)
                    {
                        thousands = digits[0].ToString() + digits[1].ToString();
                    }
                }
            }
            if (thousands != null)
            {
                messages.Add(folderNumbersStub + thousands);
                if (hundreds == null && tensAndUnits != null)
                {
                    messages.Add(folderThousandAnd);
                }
                else
                {
                    messages.Add(folderThousand);
                }
            }
            if (hundreds != null)
            {
                messages.Add(folderNumbersStub + hundreds);
                // don't always use "hundred and"
                Boolean addedHundreds = false;
                if (tensAndUnits != null)
                {
                    // if there's a thousand, or we're saying something like "13 hundred", then always use the long version
                    if (hundreds.Length == 2 || thousands != null || random.NextDouble() > 0.6)
                    {
                        messages.Add(folderHundredAnd);
                        addedHundreds = true;
                    }
                }
                else
                {
                    messages.Add(folderHundred);
                    addedHundreds = true;
                }
                if (!addedHundreds)
                {
                    if (tensAndUnits != null && tensAndUnits.Length == 1)
                    {
                        // need to modify the tensAndUnits here - we've skipped "hundreds" even though the number is > 99.
                        // This is fine if the tensAndUnits > 9 (it'll be read as "One twenty five"), but if the tensAndUnits < 10
                        // this will be read as "One two" instead of "One oh two".
                        tensAndUnits = "0" + tensAndUnits;
                    }
                }
            }
            if (tensAndUnits != null)
            {
                messages.Add(folderNumbersStub + tensAndUnits);
            }
            Console.WriteLine(String.Join(", ", messages));
            return messages;
        }
    }
}
