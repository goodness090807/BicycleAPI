namespace BicycleAPI.Application.Features.Auth.Queries.GetUserById;

public class GetUserByIdResponse
{
    public GetUserByIdResponse(Guid id, string email, string displayName)
    {
        Id = id;
        Email = email;
        DisplayName = displayName;
    }

    public Guid Id { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
}
