using MaverickBank.Models;
using Microsoft.EntityFrameworkCore;


namespace MaverickBank.Contexts
{
    public class MaverickBankContext : DbContext
    {
        public MaverickBankContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanMaster> LoanMasters { get; set; }







        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
         .HasOne(u => u.Customer)
         .WithOne(c => c.User)
         .HasForeignKey<Customer>(c => c.UserId)
         .OnDelete(DeleteBehavior.Cascade); // Deleting user deletes customer

            //now
            modelBuilder.Entity<User>()
        .HasOne(u => u.Employee)
        .WithOne(e => e.User)
        .HasForeignKey<Employee>(e => e.UserId)
        .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<User>()
        .HasOne(u => u.Admin)
        .WithOne(a => a.User)
        .HasForeignKey<Admin>(a => a.UserId)
        .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Accounts)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting customer deletes accounts

            modelBuilder.Entity<Account>()
                .HasOne(a => a.AccountType)
                .WithMany(at => at.Accounts)
                .HasForeignKey(a => a.AccountTypeId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict if account type is used

            //now
            modelBuilder.Entity<Branch>()
        .HasMany(b => b.Employees)
        .WithOne(e => e.Branch)
        .HasForeignKey(e => e.BranchId)
        .OnDelete(DeleteBehavior.Cascade);

         



        }
    }
}
