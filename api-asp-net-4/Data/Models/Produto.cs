using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_asp_net_4.Data
{
    public class Produto
    {
        [Key]
        [Required]
        [MinLength(36)]
        [MaxLength(36)]
        public string Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(80)]
        public string Nome { get; set; }

        [Required]
        [CustomValidation(typeof(Produto), "NonNegativeValue")]
        [Column(TypeName = "decimal(16, 2)")]
        public decimal Valor { get; set; }

        public static ValidationResult NonNegativeValue(decimal decimalNumber)
        {
            if (decimalNumber < 0)
                return new ValidationResult("product value should not be negative");

            return ValidationResult.Success;
        }

        [Required]
        public bool Ativo { get; set; }
    }
}
