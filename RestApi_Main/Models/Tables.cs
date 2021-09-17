using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RestApi_Main.Models
{
    public class CreditApplications
    {
        [Key]
        public int Id { get; set; }

        public string ApplicationNum { get; set; }

        public DateTime ApplicationDate { get; set; }

        public string BranchBank { get; set; }

        public string BranchBankAddr { get; set; }

        public int CreditManagerId { get; set; }

        public int ApplicantId { get; set; }

        public int RequestedCreditId { get; set; }

        public bool ScoringStatus { get; set; }

        public DateTime? ScoringDate { get; set; }
    }

    public class Clients
    {
        [Key]
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public DateTime DateBirth { get; set; }

        public string CityBirth { get; set; }

        public string AddressBirth { get; set; }

        public string AddressCurrent { get; set; }

        public int INN { get; set; }

        public int SNILS { get; set; }

        public string PassportNum { get; set; }

    }

    public class CreditManagers
    {
        [Key]
        public int CreditManagerId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

    }

    public class CreditsType
    {
        [Key]
        public int CreditType { get; set; }

        public int RequestedAmount { get; set; }

        public string RequestedCurrency { get; set; }

        public int AnnualSalary { get; set; }

        public int MonthlySalary { get; set; }

        public string CompanyName { get; set; }

        public string Comment { get; set; }

    }

    public class Currencies
    {
        [Key]
        public string CurrencyCode { get; set; }

        public string CurrencyName { get; set; }

    }

    public class DataContext : DbContext
    {
        public DbSet<CreditApplications> CreditApplications { get; set; }
        public DbSet<Clients> Clients { get; set; }
        public DbSet<CreditManagers> CreditManagers { get; set; }
        public DbSet<CreditsType> CreditsType { get; set; }
        public DbSet<Currencies> Currencies { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}
