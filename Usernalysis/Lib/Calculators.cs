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

        public static IDictionary<string, decimal> PercentagePeopleInState(IList<UserModel> users)
        {
            var total = users.Count;
            var results = users
                .GroupBy(user => user.Location.State)
                .Select(state => new { State = state.Key, Count = state.Count()})
                .OrderByDescending(state => state.Count)
                .ToDictionary(entry => entry.State, entry => (decimal)entry.Count/total);
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
    }
}
