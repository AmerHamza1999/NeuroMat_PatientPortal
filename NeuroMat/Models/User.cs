using Microsoft.AspNetCore.Identity;

namespace NeuroMat.Models
{
    public enum UserType { Patient, Clinician, Admin }
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public UserType UserType { get; set; }
    }
}
