using GymDomain.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDomain.Model
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = null!;
        public int GymId { get; set; }
    }
}

