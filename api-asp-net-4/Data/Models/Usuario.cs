using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace api_asp_net_4.Data
{
    [Index(nameof(Usuario.Email), IsUnique=true)]
    public class Usuario
    {
        [Key]
        [Required]
        [MinLength(9)]
        [MaxLength(9)]
        public string Identificacao { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Nome { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        public string Senha { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
