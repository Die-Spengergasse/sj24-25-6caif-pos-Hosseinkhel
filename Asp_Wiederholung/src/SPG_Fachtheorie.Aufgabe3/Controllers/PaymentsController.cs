using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            [FromQuery] DateTime? dateFrom)
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

        [HttpPut("paymentItems/{id}")]
        public ActionResult UpdatePaymentItem(int id, [FromBody] UpdatePaymentItemCommand command)
        {
            // Validate routing ID matches payload ID
            if (id != command.Id)
            {
                return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
                    HttpContext,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid payment item ID",
                    detail: "The ID in the route does not match the ID in the payload."));
            }

            // Find existing payment item
            var existingPaymentItem = _db.PaymentItems
                .FirstOrDefault(pi => pi.Id == id);

            if (existingPaymentItem == null)
            {
                return NotFound(ProblemDetailsFactory.CreateProblemDetails(
                    HttpContext,
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Payment Item not found",
                    detail: $"Payment Item with ID {id} could not be found."));
            }

            // Check for concurrent modification
            if (existingPaymentItem.LastUpdated != command.LastUpdated)
            {
                return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
                    HttpContext,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Payment item has changed",
                    detail: "The payment item has been modified by another user."));
            }

            // Validate payment exists
            var payment = _db.Payments.FirstOrDefault(p => p.Id == command.PaymentId);
            if (payment == null)
            {
                return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
                    HttpContext,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid payment ID",
                    detail: $"Payment with ID {command.PaymentId} could not be found."));
            }

            // Update payment item
            existingPaymentItem.ArticleName = command.ArticleName;
            existingPaymentItem.Amount = command.Amount;
            existingPaymentItem.Price = command.Price;
            existingPaymentItem.LastUpdated = DateTime.UtcNow;

            try
            {
                _db.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
                    HttpContext,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Error updating payment item",
                    detail: ex.Message));
            }
        }

        [HttpPatch("{id}")]
        public ActionResult UpdatePaymentConfirmation(int id, [FromBody] UpdateConfirmedCommand command)
        {
            // Find existing payment
            var payment = _db.Payments.FirstOrDefault(p => p.Id == id);

            if (payment == null)
            {
                return NotFound(ProblemDetailsFactory.CreateProblemDetails(
                    HttpContext,
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Payment not found",
                    detail: $"Payment with ID {id} could not be found."));
            }

            // Check if already confirmed
            if (payment.Confirmed.HasValue)
            {
                return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
                    HttpContext,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Payment already confirmed",
                    detail: "This payment has already been confirmed."));
            }

            // Update payment confirmation
            payment.Confirmed = command.Confirmed ?? DateTime.UtcNow;

            try
            {
                _db.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
                    HttpContext,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Error updating payment confirmation",
                    detail: ex.Message));
            }
        }
    }
}