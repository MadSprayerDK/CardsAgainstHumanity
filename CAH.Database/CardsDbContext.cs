using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAH.Model;

namespace CAH.Database
{
    public class CardsDbContext : DbContext
    {
        public CardsDbContext() : base("DefaultConnection")
        {}

        public DbSet<Card> Cards { set; get; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Card>().HasKey(x => x.Id);
            modelBuilder.Entity<Card>().ToTable("cah_cards");
            modelBuilder.Entity<Card>().Ignore(x => x.CardUsed);
        }
    }
}
