using SPG_Fachtheorie.Aufgabe1.Model;

namespace SPG_Fachtheorie.Aufgabe3.Dtos
{
    public record PaymentDetailDto(int id, string employeeFirstName,
        string employeeLastName, int cashDeskNumber,
        string paymentType, List<PaymentItemDto> paymentItems);
}
