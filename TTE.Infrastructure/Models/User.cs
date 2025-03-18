namespace TTE.Infrastructure.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? SecurityAnswer { get; set; } = string.Empty;

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int? SecurityQuestionId { get; set; }
        public SecurityQuestion? SecurityQuestion { get; set; }
    }
}
