using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrightIdeas.Models{
    public class Post{
        public int PostId {get; set;}
        [Required(ErrorMessage ="You must have text in your post")]
        [MinLength(2)]
        public string PostText {get; set;}
        public int CreatorId {get; set;}
        public User Creator {get; set;}
        public List<Like> Users {get; set;}
        public Post(){
            Users = new List<Like>();
        }
    }
}