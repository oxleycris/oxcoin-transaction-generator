using System;
using System.ComponentModel.DataAnnotations;

namespace OxCoin.TransactionGenerator.Data.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string GivenName { get; set; }

        [Required]
        public string FamilyName { get; set; }
    }
}
