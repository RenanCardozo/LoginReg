#pragma warning disable CS8618
#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LoginReg.Models;

public class LoginUser
{
    [Required]
    [Display(Name = "Email Address")]
    public string LEmail { get; set; }
    
    [Required]
    [Display(Name = "Password")]
    public string LPassword { get; set; }
} // Member hides inherited member; missing new keyword
