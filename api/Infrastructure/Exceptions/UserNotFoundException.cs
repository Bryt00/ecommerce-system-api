namespace Infrastructure.Exceptions
{
    public class UserNotFoundException : Exception
{
    public UserNotFoundException() : base("No User Found") { }
}
}
