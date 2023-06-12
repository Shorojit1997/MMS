﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Entities.DbSet
{
    public class Person:BaseEntity
    {
        public string Name { get; set; }

        [EmailAddress(ErrorMessage ="Invalid email address")]
        public string Email { get; set; }

        public string? Password { get; set; }

        public string Phone { get; set; }

        public string? PictureUrl { get; set; } = "/images/Default.jpg";

        public List<MessHaveMember> Members { get; set; } = new List<MessHaveMember>();

        public List<Account> Accounts { get; set; } = new List<Account>();

        public List<Deposit> Deposits { get; set; } = new List<Deposit>();

    }
}
