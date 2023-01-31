namespace API.DTOs
{
  public class UserDto  // have the properties that we want to send back when a client has successfully logged in or registered
    {
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public string Image { get; set; }
        public string Username { get; set; }
    }
}