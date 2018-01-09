using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OxCoin.TransactionGenerator.Data.Entities
{
    public class Miner
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("Wallet")]
        public Guid WalletId { get; set; }

        public virtual Wallet Wallet { get; set; }
    }
}
