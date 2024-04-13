namespace HL7_API.Model;

public class HL7DataModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CaseNumber { get; set; }
    public string Identification { get; set; }
    public string FIN { get; set; }
    public string TSAccountId { get; set; }
    public string FacilityName { get; set; }
    public string NPINumber { get; set; }
    public string PhysicianLastName { get; set; }
    public string PhysicianFirstName { get; set; }
    public DateTime DateOfConsult { get; set; }
    public string CPTCode { get; set; }
    public bool Success { get; set; } = false;
}
