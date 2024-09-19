﻿namespace Undersoft.SDK.Uniques
{
    public static class UniquePrimes
    {
        private static readonly int[] primes =
        {
            53,
            97,
            193,
            389,
            769,
            1543,
            3079,
            6151,
            12289,
            17519,
            24593,
            49157,
            75431,
            98317,
            156437,
            196613,
            270371,
            393241,
            560689,
            786433,
            1162687,
            1572869,
            2009191,
            3145739,
            4166287,
            6291469,
            7199369,
            10351711,
            13289233,
            15517591,
            17987791,
            20081053,
            22983811,
            25165843,
            34545523,
            50331653,
            76724731,
            100663319,
            154205533,
            201326611,
            309946381,
            402653189,
            622991143,
            805306457,
            1256055793,
            1610612741
        };

        public static int Get(int id)
        {
            return primes[id];
        }
    }
}
