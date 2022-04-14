using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class MemberUpdateDto
    {
        public string Introduction { get; set; }
        public string LookingFor { get; set; }        
        
        // use if field is rquired
        // [Required]
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
