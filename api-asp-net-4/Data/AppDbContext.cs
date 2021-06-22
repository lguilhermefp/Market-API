using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace api_asp_net_4.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>()
                .Property(prop => prop.Id);

            modelBuilder.Entity<Produto>()
                .Property(prop => prop.Nome);

            modelBuilder.Entity<Produto>()
                .Property(prop => prop.Valor);

            modelBuilder.Entity<Produto>()
                .Property(prop => prop.Ativo);

            modelBuilder.Entity<Produto>()
                .HasData(
                    new Produto { Id = "abcd1234-abcd-1234-abcd1234-abcd1234", Nome = "produto-1", Valor = 10, Ativo = true });


            modelBuilder.Entity<Usuario>()
                .Property(prop => prop.Identificacao);

            modelBuilder.Entity<Usuario>()
                .Property(prop => prop.Nome);

            modelBuilder.Entity<Usuario>()
                .Property(prop => prop.Email);

            modelBuilder.Entity<Usuario>()
                .Property(prop => prop.Senha);

            modelBuilder.Entity<Usuario>()
                .HasData(
                    new Usuario { Identificacao = "admin-123", Nome = "admin", Email = "admin@example.com", Senha = "V1ZkU2RHRlhOSGhOYWsw" }); //Senha descriptografada = "admin123"
        }
    }
}
