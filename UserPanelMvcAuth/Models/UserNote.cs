namespace UserPanelMvcAuth.Models
{
    public class UserNote
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public AppUser AppUser { get; set; } = null!;
    }
}
