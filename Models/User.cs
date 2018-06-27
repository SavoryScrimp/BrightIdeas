using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrightIdeas.Models{
    public class User{
        public int UserId {get; set;}
        
        [Required(ErrorMessage = "Name must be at least 2 characters")]
        [MinLength(3)]
        public string Name {get; set;}
        
        [Required(ErrorMessage = "Alias must be at least 1 character")]
        [MinLength(3)]
        public string Alias {get; set;}
        [Required(ErrorMessage = "Email must be in proper Email format")]
        [EmailAddress]
        public string Email {get; set;}
        [MinLength(8)]
        [Required(ErrorMessage = "Password must be 8 or more characters")]
        public string Password {get; set;}

        public List<Like> Posts {get; set;}
        public User(){
            Posts = new List<Like>();
        }
    }
}