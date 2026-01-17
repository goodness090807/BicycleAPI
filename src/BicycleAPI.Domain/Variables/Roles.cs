namespace BicycleAPI.Domain.Variables;

public static class Roles
{
    public static readonly Guid MemberId = Guid.Parse("019479a0-0001-7000-8000-000000000001");
    public static readonly Guid TechnicianId = Guid.Parse("019479a0-0002-7000-8000-000000000002");

    public const string MemberCode = "MEMBER";
    public const string TechnicianCode = "TECHNICIAN";

    public const string MemberName = "Member";
    public const string TechnicianName = "Technician";
}
