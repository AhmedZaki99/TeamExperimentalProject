using System.ComponentModel.DataAnnotations;

namespace DataProcessingCore
{
    public class UserOutputDto : OutputDtoBase
    {
        [Display(Name = "User Name")]
        public string? UserName { get; set; }
        
        [Display(Name = "Email")]
        public string? Email { get; set; }


        [Display(Name = "First Name")]
        public string? FirstName { get; set; }
        
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }


        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }
    }
}
