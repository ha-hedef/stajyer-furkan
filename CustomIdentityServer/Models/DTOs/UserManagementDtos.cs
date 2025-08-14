using System.ComponentModel.DataAnnotations;

namespace CustomIdentityServer.Models.DTOs
{
    public class AssignRoleDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string RoleName { get; set; } = string.Empty;
    }

    public class UpdateProfileDto
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class LockUserDto
    {
        public bool Lock { get; set; }
        public int DaysLocked { get; set; }
    }

    public class UserFilterDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsLocked { get; set; }
        public bool? EmailConfirmed { get; set; }
    }
}