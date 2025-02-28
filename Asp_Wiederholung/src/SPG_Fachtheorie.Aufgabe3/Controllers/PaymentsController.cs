using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SPG_Fachtheorie.Aufgabe3.Dtos;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private static readonly List<PaymentDto> Payments = new List<PaymentDto>
    {
        new PaymentDto { Id = 1, EmployeeFirstName = "John", EmployeeLastName = "Doe", CashDeskNumber = 1, PaymentType = "Cash", TotalAmount = 100.50m },
        new PaymentDto { Id = 2, EmployeeFirstName = "Jane", EmployeeLastName = "Smith", CashDeskNumber = 2, PaymentType = "Card", TotalAmount = 200.75m },
        // Weitere Dummy-Daten können hier hinzugefügt werden.
    };

    private static readonly List<PaymentDetailDto> PaymentDetails = new List<PaymentDetailDto>
    {
        new PaymentDetailDto
        {
            Id = 1,
            EmployeeFirstName = "John",
            EmployeeLastName = "Doe",
            CashDeskNumber = 1,
            PaymentType = "Cash",
            PaymentItems = new List<PaymentItemDto>
            {
                new PaymentItemDto { ArticleName = "Item1", Amount = 2, Price = 50.25m },
                new PaymentItemDto { ArticleName = "Item2", Amount = 1, Price = 25.00m }
            }
        },
        // Weitere Dummy-Daten können hier hinzugefügt werden.
    };

    [HttpGet]
    public IActionResult GetPayments([FromQuery] int? cashDesk, [FromQuery] DateTime? dateFrom)
    {
        var result = Payments.AsQueryable();

        if (cashDesk.HasValue)
        {
            result = result.Where(p => p.CashDeskNumber == cashDesk.Value);
        }

        if (dateFrom.HasValue)
        {
            // Hier müsste ein Datum in den Daten berücksichtigt werden (Dummy-Daten enthalten kein Datum).
            // Beispiel: result = result.Where(p => p.Date >= dateFrom.Value);
        }

        return Ok(result.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetPaymentById(int id)
    {
        var paymentDetail = PaymentDetails.FirstOrDefault(p => p.Id == id);
        if (paymentDetail == null)
        {
            return NotFound();
        }

        return Ok(paymentDetail);
    }
}
