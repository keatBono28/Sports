using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

/* 
 * Author: Keaton Bonomo
 * Update: 6/19/2019
 */

namespace BKBSports.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public virtual UserProfile UserProfile { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class UserProfile
    {
        //-- UserId --//
        [Key]
        public int userId { get; set; }

        //-- Profile First Name --//
        [Display(Name = "First Name")]
        [StringLength(30)]
        [Required(ErrorMessage = "First name is required!")]
        public string firstName { get; set; }

        //-- Profile Last Name --//
        [Display(Name = "Last Name")]
        [StringLength(30)]
        [Required(ErrorMessage = "Last name is required!")]
        public string lastName { get; set; }

        //-- Profile Preferred Name --//
        [Display(Name = "Preffered Name")]
        [StringLength(30)]
        public string preferredName { get; set; }

        //-- Profile Date of Birth --//
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date of Birth is required! -> MM/DD/YYYY")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyy}", ApplyFormatInEditMode = true)]
        public DateTime dateOfBirth { get; set; }

        //-- Profile Phone Number --// 
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\(\d{3}\) |\d{3}-)\d{3}-\d{4}$", ErrorMessage = "Phone Number must be in xxx-xxx-xxxx or (xxx)-xxx-xxxx format!")]
        [Required(ErrorMessage ="A phone number is required")]
        public string phoneNumber { get; set; }

        //-- Profile Creation Date --//
        [Display(Name = "Profile Creation Date")]
        public DateTime profileCreationDate { get; set; }

        //-- Profile Update Timestamp --//
        [Display(Name = "Profile Update Timestamp")]
        public DateTime profileUpdateTimestamp { get; set; }

        //-- Profile Image --//
        public byte[] profileImage { get; set; }

        //-- Profile Type --//
        public ProfileType profileType { get; set; }
    }

    // Enum of Profile Types
    public enum ProfileType
    {
        Public = 1,
        Writer = 2,
        Admin = 3
    }
    




    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<BKBSports.Models.UserProfile> UserProfiles { get; set; }
    }
}