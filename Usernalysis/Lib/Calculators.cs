using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usernalysis.Models.User;

namespace Usernalysis.Lib
{
    public class Calculators
    {
        private const char DEFAULT_NAME_MIDPOINT = 'm';
        private static readonly int[] DEFAULT_AGE_SPLITS = { 20, 40, 60, 80, 100 };

        public static decimal PercentageFemale(IList<UserModel> users)
        {
            var femaleCount = (from user in users
                               where user.Gender == Gender.female
                               select user).Count();
            return (decimal)femaleCount / users.Count;
        }

        public static decimal PercentageFirstNameMidpoint(IList<UserModel> users, char midPoint = DEFAULT_NAME_MIDPOINT)
        {
            return PercentageNameMidpoint(users, midPoint, true);
        }

        public static decimal PercentageLastNameMidpoint(IList<UserModel> users, char midPoint = DEFAULT_NAME_MIDPOINT)
        {
            return PercentageNameMidpoint(users, midPoint, false);
        }

        public static IDictionary<string, decimal> PercentageFemalesInState(IList<UserModel> users)
        {
            return PercentagePeopleInState(users, Gender.female);
        }

        public static IDictionary<string, decimal> PercentageMalesInState(IList<UserModel> users)
        {
            return PercentagePeopleInState(users, Gender.male);
        }

        public static IDictionary<string, decimal> PercentagePeopleInState(IList<UserModel> users, Gender? gender = null)
        {
            var query = (from user in users select user);
            if (gender.HasValue)
            {
                if (gender.Value == Gender.female)
                {
                    query = query.Where(user => user.Gender == Gender.female);
                }
                else
                {
                    query = query.Where(user => user.Gender == Gender.male);
                }
            }
            var total = query.Count();
            var results = query.GroupBy(user => user.Location.State)
                .Select(state => new { State = state.Key, Count = state.Count() })
                .OrderByDescending(state => state.Count)
                .ToDictionary(entry => entry.State, entry => (decimal)entry.Count/total);
            return results;
        }

        public static IList<KeyValuePair<string, decimal>> PercentageAgeRanges(IList<UserModel> users, int[] ageSplits = null)
        {
            // Since ageSplits is not a value type, we cannot assign it a default value in the parameter.  As a workaround,
            // check if ageSplits is the defaulted null value and assign the default value if it is null.
            ageSplits = ageSplits ?? DEFAULT_AGE_SPLITS;

            var results = new List<KeyValuePair<string, decimal>>();
            var total = users.Count;
            var orderedAgeSplits = ageSplits.OrderBy(age => age).ToArray();
            var orderedAges = users.OrderBy(u => u.Dob.Age)
                .Select(user => user.Dob.Age).ToArray();
            int currentSplit = 0;
            int currentRangeCount = 0;

            // Ignore negative ages in orderedAgeSplits
            while(orderedAgeSplits[currentSplit] <= 0)
            {
                currentSplit++;
            }

            for(int i=0; i<orderedAges.Length; i++)
            {
                int age = orderedAges[i];
                if (age <= orderedAgeSplits[currentSplit])
                {
                    currentRangeCount++;
                }
                else
                {
                    results.Add(new KeyValuePair<string, decimal>(GetRangeLabel(orderedAgeSplits, currentSplit), (decimal)currentRangeCount / total));
                    currentRangeCount = 1;
                    currentSplit++;
                    while(currentSplit < orderedAgeSplits.Length && age > orderedAgeSplits[currentSplit])
                    {
                        results.Add(new KeyValuePair<string, decimal>($"{GetRangeLabel(orderedAgeSplits, currentSplit)}", 0));
                        currentSplit++;
                    }
                    if(currentSplit >= orderedAgeSplits.Length)
                    {
                        currentRangeCount = 0;
                        results.Add(new KeyValuePair<string, decimal>($"{GetRangeLabel(orderedAgeSplits, currentSplit)}", (decimal)(total - i) / total));
                        break;
                    }
                }
            }

            if (currentRangeCount > 0)
            {
                results.Add(new KeyValuePair<string, decimal>($"{GetRangeLabel(orderedAgeSplits, currentSplit)}", (decimal)currentRangeCount / total));
                for (int j = currentSplit + 1; j < ageSplits.Length; j++)
                {
                    results.Add(new KeyValuePair<string, decimal>($"{GetRangeLabel(orderedAgeSplits, j)}", 0));
                }
                results.Add(new KeyValuePair<string, decimal>($"{GetRangeLabel(orderedAgeSplits, orderedAgeSplits.Length)}", 0));
            }
            return results;
        }

        private static decimal PercentageNameMidpoint(IList<UserModel> users, char midPoint, bool useFirstName)
        {
            var midPointStr = ((char)(midPoint + 1)).ToString();
            var leftCountQuery = (from user in users select user);
            if (useFirstName)
            {
                leftCountQuery = leftCountQuery.Where(user => user.Name.First.CompareTo(midPointStr) < 0);
            }
            else
            {
                leftCountQuery = leftCountQuery.Where(user => user.Name.Last.CompareTo(midPointStr) < 0);
            }
            var leftCount = leftCountQuery.Count();
            return (decimal)leftCount / users.Count;
        }

        private static string GetRangeLabel(int[] splits, int index)
        {
            string label = string.Empty;
            if (index >= splits.Length)
            {
                label = $"{splits[splits.Length - 1] + 1}+";
            }
            else {
                label = $"{splits[index]}";
                if (index == 0)
                {
                    label = $"0-{label}";
                }
                else
                {
                    label = $"{splits[index - 1] + 1}-{label}";
                }
            }
            return label;
        }
    }
}
