using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Usernalysis.Models.User
{
    // ASSUMPTION: User gender is only Female & Male.
    public enum Gender { female, male }

    public class UserModel
    {
        public Gender Gender { get; set; }
        public NameModel Name { get; set; }
        public LocationModel Location { get; set; }
        public BirthDateModel Dob { get; set; }
    }
}
