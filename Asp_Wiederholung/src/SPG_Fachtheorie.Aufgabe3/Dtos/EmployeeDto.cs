using SPG_Fachtheorie.Aufgabe1.Model;

namespace SPG_Fachtheorie.Aufgabe3.Dtos
{
    public record EmployeeDto(
        int RegistrationNumber, string Firstname,
        string Lastname, string Longname,
        string Type);
}
