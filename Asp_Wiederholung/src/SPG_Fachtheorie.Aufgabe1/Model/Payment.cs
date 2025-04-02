using System;
using System.Collections.Generic;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Payment
    {
        private DateTime dateTime;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected Payment() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public Payment(CashDesk cashDesk, DateTime paymentDateTime, Employee employee, PaymentType paymentType)
        {
            CashDesk = cashDesk;
            PaymentDateTime = paymentDateTime;
            Employee = employee;
            PaymentType = paymentType;
        }

        public Payment(CashDesk cashDesk, DateTime dateTime, Cashier employee)
        {
            CashDesk = cashDesk;
            this.dateTime = dateTime;
            Employee = employee;
        }

        public int Id { get; set; }
        public CashDesk CashDesk { get; set; }
        public DateTime PaymentDateTime { get; set; }
        public Employee Employee { get; set; }
        public PaymentType PaymentType { get; set; }
        public List<PaymentItem> PaymentItems { get; } = new();
        public object Confirmed { get; set; }
    }
}