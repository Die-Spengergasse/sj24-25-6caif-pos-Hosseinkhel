namespace SPG_Fachtheorie.Aufgabe3.Dtos
{
    public class PaymentDetailDto
    {
        public int Id { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public int CashDeskNumber { get; set; }
        public string PaymentType { get; set; }
        public List<PaymentItemDto> PaymentItems { get; set; } = new List<PaymentItemDto>();
    }

}
