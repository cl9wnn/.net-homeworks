namespace Core.Entities;

public class UserRegistration
{
    public Guid UserId  { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime RegisteredAt { get; set; }
}