
using SPG_Fachtheorie.Aufgabe1.Model;

namespace SPG_Fachtheorie.Aufgabe3.Dtos
{
    public record EmployeeDetailDto(
        int RegistrationNumber, string Firstname,
        string Lastname, string Longname,
        Address? Address,
        string Type);
}
