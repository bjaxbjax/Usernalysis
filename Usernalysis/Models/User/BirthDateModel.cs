using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Usernalysis.Models.User
{
    public class BirthDateModel
    {
        public DateTime Date { get; set; }
        public int Age {
            get
            {
                TimeSpan diff = DateTime.Now - this.Date;
                return (int)diff.TotalDays / 365;
            }
        }
    }
}
