using System;

namespace Opus_ChatBot_HealthCare_8.Helpers
{
    public class TextMatcher
    {
        public static double CalculateLevenshteinSimilarity(string text1, string text2)
        {
            int maxLen = Math.Max(text1.Length, text2.Length);
            if (maxLen == 0)
                return 1.0;  // Both strings are empty, consider them similar

            int levenshteinDistance = CalculateLevenshteinDistance(text1, text2);

            // Normalize to a similarity score between 0 and 1
            double similarity = 1.0 - (double)levenshteinDistance / maxLen;

            return similarity;
        }

        private static int CalculateLevenshteinDistance(string s1, string s2)
        {
            int[,] distance = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                distance[i, 0] = i;

            for (int j = 0; j <= s2.Length; j++)
                distance[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                    distance[i, j] = Math.Min(
                        Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                        distance[i - 1, j - 1] + cost
                    );
                }
            }

            return distance[s1.Length, s2.Length];
        }
    }
}
