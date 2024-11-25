using Microsoft.EntityFrameworkCore;
using Producao.Models;

namespace Producao.Data
{
    public class ProducaoContext : DbContext
    {
        public ProducaoContext(DbContextOptions<ProducaoContext> options) : base(options)
        {
            
        }

        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DB_Producao;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public DbSet<Maquina> Maquinas { get; set; }
        public DbSet<Forma> Formas { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<MateriaPrima> MateriasPrimas { get; set; }
        public DbSet<ProcessoProducao> Producoes { get; set; }
        public DbSet<ProducaoMateriaPrima> ProducoesMateriasPrimas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<Maquina>()
                .HasMany(m => m.Formas)
                .WithMany(f => f.Maquinas);

            modelBuilder.Entity<Forma>()
                .HasOne(f => f.Produto)
                .WithMany(p => p.Formas)
                .HasForeignKey(f => f.ProdutoId)
                .IsRequired();

            modelBuilder.Entity<ProcessoProducao>()
                .HasOne(p => p.Maquina)
                .WithMany(m => m.Producoes)
                .HasForeignKey(p => p.MaquinaId)
                .IsRequired();

            modelBuilder.Entity<ProcessoProducao>()
                .HasOne(p => p.Forma)
                .WithMany(f => f.Producoes)
                .HasForeignKey(p => p.FormaId)
                .IsRequired();

            modelBuilder.Entity<ProcessoProducao>()
                .HasOne(p => p.Produto)
                .WithMany(p => p.Producoes)
                .HasForeignKey(p => p.ProdutoId);

            modelBuilder.Entity<ProducaoMateriaPrima>()
                .HasKey(pp => new { pp.ProducaoId, pp.MateriaPrimaId });

            modelBuilder.Entity<ProducaoMateriaPrima>()
                .HasOne(pp => pp.ProcessoProducao)
                .WithMany(p => p.ProducaoMateriasPrimas)
                .HasForeignKey(pp => pp.ProducaoId);

            modelBuilder.Entity<ProducaoMateriaPrima>()
                .HasOne(pp => pp.MateriaPrima)
                .WithMany(p => p.ProducaoMateriasPrimas)
                .HasForeignKey(pp => pp.MateriaPrimaId);

        }
    }
}
