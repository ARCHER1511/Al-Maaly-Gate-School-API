using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Application.Authentication
{
    public static class JwtExtensions
    {
        public static string GenerateJwtToken(
            AppUser user,
            Teacher? teacher,
            IList<string> roles,
            IConfiguration _config
        )
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            //Custom claims section
            claims.Add(new Claim("AccountStatus", user.AccountStatus.ToString()));
            if (roles.Contains("Teacher"))
            {
                if (teacher != null &&teacher!.TeacherSubjects != null && teacher.TeacherSubjects.Any())
                {
                    var subjectIdentifiers = teacher.TeacherSubjects.Select(ts => ts.Subject.SubjectName).ToList();
                    var subjectJson = JsonSerializer.Serialize(subjectIdentifiers);
                    claims.Add(new Claim("TeacherSubjects",subjectJson));

                    var classIdentifier = teacher!.TeacherClasses.Select(tc => tc.Class.ClassName).ToList();
                    var classJson = JsonSerializer.Serialize(classIdentifier);
                    claims.Add(new Claim("TeacherClasses",classJson));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(double.Parse(_config["Jwt:DurationInDays"]!));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
