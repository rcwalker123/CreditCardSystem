﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CreditCardSystem.Models
{
    public partial class CreditCard
    {
        public Guid CreditCardId { get; set; }
        [Display(Name = "Card Number")]
        [Required]
        [DataType(DataType.CreditCard)]
        public string CardNumber { get; set; }
        [Display(Name = "Card Expiry")]
        [Required]
        [DataType(DataType.Date, ErrorMessage = "Date is required")]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CardExpiry { get; set; }
        [Display(Name = "Card CVV")]
        [Required]
        public string CardCvv { get; set; }
        public Guid CardTypeId { get; set; }

        [Display(Name = "Card Type")]
        [Required]
        public virtual CardType CardType { get; set; }
    }
}