﻿using System;
using System.ComponentModel.DataAnnotations;

namespace OxCoin.TransactionGenerator.Data.Entities
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid SourceWalletId { get; set; }

        [Required]
        public Guid DestinationWalletId { get; set; }

        [Required]
        public decimal TransferedAmount { get; set; }

        [Required]
        public decimal TransferFee { get; set; } = Math.Round(new Random().Next(1, 99) * 0.000001m, 8);

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public int Size { get; set; } = new Random().Next(200, 700);
    }
}
