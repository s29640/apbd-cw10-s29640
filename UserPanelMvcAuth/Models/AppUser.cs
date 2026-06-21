namespace UserPanelMvcAuth.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<UserNote> Notes { get; set; } = new List<UserNote>();
    }
}
