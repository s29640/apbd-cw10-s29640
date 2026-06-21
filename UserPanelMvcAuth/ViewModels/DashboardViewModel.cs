
using System.ComponentModel.DataAnnotations;
using UserPanelMvcAuth.Models;

namespace UserPanelMvcAuth.ViewModels
{
    public class DashboardViewModel
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(4000)]
        public string Content { get; set; } = string.Empty;

        public List<UserNote> Notes { get; set; } = new();
    }
}
