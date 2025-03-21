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
    private object _db;

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

    [HttpDelete("{id}")]
    public ActionResult DeletePayment(int id, [FromQuery] bool deleteItems = false)
    {
        try
        {
            // Find the payment
            var payment = _db.Payments
                .Include(p => p.PaymentItems)
                .FirstOrDefault(p => p.Id == id);

            if (payment == null)
            {
                return NotFound(ProblemDetailsFactory.CreateProblemDetails(
                    HttpContext,
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Payment not found",
                    detail: $"Payment with ID {id} could not be found."));
            }

            // Check if payment has items and deleteItems is false
            if (!deleteItems && payment.PaymentItems.Any())
            {
                return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
                    HttpContext,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Cannot delete payment",
                    detail: "Payment has payment items."));
            }

            // If deleteItems is true, remove all payment items first
            if (deleteItems && payment.PaymentItems.Any())
            {
                _db.PaymentItems.RemoveRange(payment.PaymentItems);
            }

            // Remove the payment
            _db.Payments.Remove(payment);
            _db.SaveChanges();

            // Return 204 No Content for successful deletion
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
                HttpContext,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Error deleting payment",
                detail: ex.Message));
        }
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
    [HttpPost]
    public ActionResult CreatePayment([FromBody] NewPaymentCommand command)
{
    // Validate payment date
    if (!command.IsPaymentDateTimeValid())
    {
        return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
            HttpContext,
            statusCode: StatusCodes.Status400BadRequest,
            title: "Invalid payment date",
            detail: "Payment date cannot be more than 1 minute in the future."));
    }

    try
    {
        // Find the cash desk by number
        var cashDesk = _db.CashDesks.FirstOrDefault(c => c.Number == command.CashDeskNumber);
        if (cashDesk == null)
        {
            return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
                HttpContext,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid cash desk",
                detail: $"Cash desk with number {command.CashDeskNumber} not found."));
        }

        // Find the employee by registration number
        var employee = _db.Employees.FirstOrDefault(e => 
            e.RegistrationNumber == command.EmployeeRegistrationNumber);
        if (employee == null)
        {
            return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
                HttpContext,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid employee",
                detail: $"Employee with registration number {command.EmployeeRegistrationNumber} not found."));
        }

        // Parse payment type from string to enum
        if (!Enum.TryParse<PaymentType>(command.PaymentType, true, out var paymentType))
        {
            return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
                HttpContext,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid payment type",
                detail: $"Payment type '{command.PaymentType}' is not valid. Valid values are: {string.Join(", ", Enum.GetNames<PaymentType>())}"));
        }

        // Create new payment
        var payment = new Payment(cashDesk, command.PaymentDateTime, employee, paymentType);
        
        // Save to database
        _db.Payments.Add(payment);
        _db.SaveChanges();

        // Return 201 Created with payment ID
        return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment.Id);
    }
    catch (Exception ex)
    {
        return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
            HttpContext,
            statusCode: StatusCodes.Status400BadRequest,
            title: "Error creating payment",
            detail: ex.Message));
    }
}
}
