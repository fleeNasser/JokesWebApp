using Microsoft.AspNetCore.Identity;

namespace JokesWebApp.Models
{
    public class Joke
    {
        public int JokeID { get; set; }

        public string JokeQuestion { get; set; }
        public string JokeAnswer { get; set; }

        public IdentityUser User { get; set; }
        public string UserId { get; set; }

        public Joke()
        {
            
        }
    }
}
