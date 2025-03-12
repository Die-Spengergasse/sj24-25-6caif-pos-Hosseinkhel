using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe3.Dtos;

namespace SPG_Fachtheorie.Aufgabe3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly AppointmentContext _db;

        public PaymentsController(AppointmentContext db)
        {
            _db = db;
        }
        [HttpGet("{id}")]
        public ActionResult<PaymentDetailDto> GetPayment(int id)
        {
            var payment = _db.Payments
                .Where(p => p.Id == id)
                .Select(p => new PaymentDetailDto(
                p.Id,
                p.Employee.FirstName,
                p.Employee.LastName,
                p.CashDesk.Number,
                p.PaymentType.ToString(),
                p.PaymentItems.Select(pi => new PaymentItemDto(
                    pi.ArticleName,
                    pi.Amount,
                    pi.Price))
                .ToList()
                ))
                .FirstOrDefault();
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }
        [HttpGet]
       public ActionResult<List<PaymentDto>> GetPayments(
    [FromQuery] int? cashDesk,
    [FromQuery] DateTime? dateFrom
)
        {
            IQueryable<Payment> query = _db.Payments;

            if (cashDesk.HasValue)
            {
                query = query.Where(p => p.CashDesk.Number == cashDesk.Value);
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(p => p.PaymentDateTime >= dateFrom.Value);
            }

            var payments = query
                .Select(p => new PaymentDto(
                    p.Id,
                    p.Employee.FirstName,
                    p.Employee.LastName,
                    p.CashDesk.Number,
                    p.PaymentType.ToString(),
                    (int)p.PaymentItems.Sum(pi => pi.Price * pi.Amount)
                ))
                .ToList();

            return Ok(payments);
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
}
