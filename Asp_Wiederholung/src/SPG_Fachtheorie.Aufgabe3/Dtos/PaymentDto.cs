namespace SPG_Fachtheorie.Aufgabe3.Dtos
{
    public record PaymentDto(int id, string employeeFirstName,
        string employeeLastName, int cashDeskNumber, 
        string paymentType, int totalAmount);
}
