using Microsoft.AspNetCore.Identity;

namespace BookReviewApp.Models.Domain
{
    public class User : IdentityUser
    {
        public virtual ICollection<Review> Reviews { get; set; } = [];
        public virtual ICollection<ReviewVote> ReviewVotes { get; set; } = [];
    }
}