namespace UserManagement.Data.Models.Entities;

public class Student
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTimeOffset BirthDate { get; set; }
}
