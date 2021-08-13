using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication.Authentication;

namespace WebApplication.Models
{
    public class SinglePurposeUserGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ApplicationUser> GroupUsers { get; set; }
        public virtual ICollection<UnidirectionalPaymentGroup> GroupPayments { get; set; }
    }
}