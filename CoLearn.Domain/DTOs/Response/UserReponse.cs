using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs.Response
{
    public class UserReponse
    {
        public record GetUserModel(
            int UserId,
            string FullName,
            string Email,
            string? Phone,
            DateTime? DateOfBirth,
            string? Gender,
            int? PrimaryRoleId,
            bool IsActive,
            bool IsDeleted,
            DateTime? DeletedAt,
            DateTime CreatedAt,
            DateTime? UpdatedAt
        );
        public class Login
        {
            public string Token { get; set; } = default!;
            public DateTime Expiration { get; set; }
            public string Role { get; set; } = default!;
            public string FullName { get; set; } = default!;
            public int UserId { get; set; }
    }

}
}
