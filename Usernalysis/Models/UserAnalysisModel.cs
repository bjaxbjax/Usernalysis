using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Usernalysis.Models
{
    public class UserAnalysisModel
    {
        public (decimal PercentFemale, decimal PercentMale) Gender { get; set; }
        public (decimal PercentAtoM, decimal PercentNtoZ) FirstName { get; set; }
        public (decimal PercentAtoM, decimal PercentNtoZ) LastName { get; set; }
        public IDictionary<string, decimal> StatePercentages { get; set; }
        public IDictionary<string, decimal> FemaleStatePercentages { get; set; }
        public IDictionary<string, decimal> MaleStatePercentages { get; set; }
        public IList<KeyValuePair<string, decimal>> AgeRangePercentages { get; set; }
    }
}
