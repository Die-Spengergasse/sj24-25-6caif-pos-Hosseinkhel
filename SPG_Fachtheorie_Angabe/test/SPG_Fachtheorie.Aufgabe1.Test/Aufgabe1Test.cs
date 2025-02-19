using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using System;
using System.Linq;
using Xunit;

namespace SPG_Fachtheorie.Aufgabe1.Test
{
    [Collection("Sequential")]
    public class Aufgabe1Test
    {
        private AppointmentContext GetEmptyDbContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite(@"Data Source=cash.db")
                .Options;

            var db = new AppointmentContext(options);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            return db;
        }

        // Erstellt eine leere Datenbank in Debug\net8.0\cash.db
        [Fact]
        public void CreateDatabaseTest()
        {
            using var db = GetEmptyDbContext();
        }

        [Fact]
        public void AddCashierSuccessTest()
        {
            using var db = GetEmptyDbContext();

            // Erstelle eine Adresse und einen Cashier
            var address = new Address("Main Street", "Springfield", "12345");
            var cashier = new Cashier("C001", "John", "Doe", address, DateTime.Now);

            // Füge den Cashier zur Datenbank hinzu
            db.Cashiers.Add(cashier);
            db.SaveChanges();

            // Lösche den Change Tracker, um sicherzustellen, dass die Daten aus der Datenbank gelesen werden
            db.ChangeTracker.Clear();

            // Überprüfe, ob der Cashier korrekt gespeichert wurde
            var savedCashier = db.Cashiers.FirstOrDefault(c => c.RegistrationNumber == "C001");
            Assert.NotNull(savedCashier);
            Assert.Equal("C001", savedCashier.RegistrationNumber);
            Assert.Equal("John", savedCashier.FirstName);
            Assert.Equal("Doe", savedCashier.LastName);
        }

        [Fact]
        public void AddPaymentSuccessTest()
        {
            using var db = GetEmptyDbContext();

            // Erstelle eine Zahlung
            var payment = new Payment(1, DateTime.Now, 100.00m);

            // Füge die Zahlung zur Datenbank hinzu
            db.Payments.Add(payment);
            db.SaveChanges();

            // Lösche den Change Tracker, um sicherzustellen, dass die Daten aus der Datenbank gelesen werden
            db.ChangeTracker.Clear();

            // Überprüfe, ob die Zahlung korrekt gespeichert wurde
            var savedPayment = db.Payments.FirstOrDefault(p => p.Id == 1);
            Assert.NotNull(savedPayment);
            Assert.Equal(100.00m, savedPayment.TotalAmount);
        }

        [Fact]
        public void EmployeeDiscriminatorSuccessTest()
        {
            using var db = GetEmptyDbContext();

            // Erstelle eine Adresse und einen Cashier
            var address = new Address("Main Street", "Springfield", "12345");
            var cashier = new Cashier("C001", "John", "Doe", address, DateTime.Now);

            // Füge den Cashier zur Datenbank hinzu
            db.Cashiers.Add(cashier);
            db.SaveChanges();

            // Lösche den Change Tracker, um sicherzustellen, dass die Daten aus der Datenbank gelesen werden
            db.ChangeTracker.Clear();

            // Überprüfe, ob der Discriminator korrekt gesetzt wurde
            var savedEmployee = db.Employees.FirstOrDefault(e => e.RegistrationNumber == "C001");
            Assert.NotNull(savedEmployee);
            Assert.Equal("Cashier", savedEmployee.Type);
        }
    }
}