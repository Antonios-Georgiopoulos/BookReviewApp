using BookReviewApp.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            if (context.Books.Any())
            {
                return;
            }

            var users = await CreateUsersAsync(userManager);

            var books = CreateBooks();
            context.Books.AddRange(books);
            await context.SaveChangesAsync();

            var reviews = CreateReviews(books, users);
            context.Reviews.AddRange(reviews);
            await context.SaveChangesAsync();

            var votes = CreateReviewVotes(reviews, users);
            context.ReviewVotes.AddRange(votes);
            await context.SaveChangesAsync();
        }

        private static async Task<List<User>> CreateUsersAsync(UserManager<User> userManager)
        {
            var users = new List<User>();

            var testUsers = new[]
            {
                new { Email = "admin@bookreview.com", UserName = "admin", Password = "Admin123!" },
                new { Email = "user1@bookreview.com", UserName = "bookworm", Password = "User123!" },
                new { Email = "user2@bookreview.com", UserName = "reader", Password = "User123!" },
                new { Email = "user3@bookreview.com", UserName = "critic", Password = "User123!" }
            };

            foreach (var userData in testUsers)
            {
                var user = new User
                {
                    UserName = userData.UserName,
                    Email = userData.Email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, userData.Password);
                if (result.Succeeded)
                {
                    users.Add(user);
                }
            }

            return users;
        }

        private static List<Book> CreateBooks()
        {
            return
            [
                new() {
                    Title = "Το Όνομα του Ρόδου",
                    Author = "Ουμπέρτο Έκο",
                    PublishedYear = 1980,
                    Genre = "Μυστήριο",
                    DateCreated = DateTime.UtcNow.AddDays(-30)
                },
                new() {
                    Title = "1984",
                    Author = "Τζορτζ Όργουελ",
                    PublishedYear = 1949,
                    Genre = "Επιστημονική Φαντασία",
                    DateCreated = DateTime.UtcNow.AddDays(-25)
                },
                new() {
                    Title = "Εκατό Χρόνια Μοναξιά",
                    Author = "Γκαμπριέλ Γκαρσία Μάρκες",
                    PublishedYear = 1967,
                    Genre = "Μυθιστόρημα",
                    DateCreated = DateTime.UtcNow.AddDays(-20)
                },
                new() {
                    Title = "Ο Αρχόντος των Δαχτυλιδιών",
                    Author = "Τζ. Ρ. Ρ. Τόλκιν",
                    PublishedYear = 1954,
                    Genre = "Φαντασίας",
                    DateCreated = DateTime.UtcNow.AddDays(-15)
                },
                new() {
                    Title = "Καλοκαιρινές Νότες για Χειμωνιάτικα Όνειρα",
                    Author = "Τίτος Πατρίκιος",
                    PublishedYear = 1987,
                    Genre = "Ποίηση",
                    DateCreated = DateTime.UtcNow.AddDays(-10)
                },
                new() {
                    Title = "Ο Μικρός Πρίγκιπας",
                    Author = "Αντουάν ντε Σαιντ-Εξυπερί",
                    PublishedYear = 1943,
                    Genre = "Παιδικό",
                    DateCreated = DateTime.UtcNow.AddDays(-5)
                },
                new() {
                    Title = "Σαπίενς: Μια Σύντομη Ιστορία της Ανθρωπότητας",
                    Author = "Γιουβάλ Νόα Χαράρι",
                    PublishedYear = 2011,
                    Genre = "Ιστορικό",
                    DateCreated = DateTime.UtcNow.AddDays(-3)
                },
                new() {
                    Title = "Η Τέχνη της Ευτυχίας",
                    Author = "Δαλάι Λάμα",
                    PublishedYear = 1998,
                    Genre = "Φιλοσοφία",
                    DateCreated = DateTime.UtcNow.AddDays(-1)
                }
            ];
        }

        private static List<Review> CreateReviews(List<Book> books, List<User> users)
        {
            var reviews = new List<Review>();
            var random = new Random();

            var sampleReviews = new[]
            {
                "Εξαιρετικό βιβλίο! Με κράτησε σε αγωνία μέχρι το τέλος. Η πλοκή είναι πολύπλοκη αλλά συνάμα κατανοητή, και οι χαρακτήρες είναι καλά ανεπτυγμένοι.",
                "Ένα από τα καλύτερα βιβλία που έχω διαβάσει φέτος. Η γραφή του συγγραφέα είναι μαγευτική και το θέμα πολύ επίκαιρο.",
                "Αρκετά καλό, αλλά περίμενα κάτι περισσότερο. Κάποιες σελίδες ήταν λίγο βαρετές, όμως συνολικά άξιζε τον κόπο.",
                "Συγκλονιστικό έργο! Αυτό το βιβλίο άλλαξε την οπτική μου για πολλά πράγματα. Το συνιστώ ανεπιφύλακτα.",
                "Δεν με εντυπωσίασε ιδιαίτερα. Η ιστορία είναι προβλέψιμη και οι χαρακτήρες στερεότυποι.",
                "Φανταστική ανάγνωση! Οι περιγραφές είναι τόσο ζωντανές που νιώθεις σαν να βρίσκεσαι μέσα στην ιστορία.",
                "Ενδιαφέρον βιβλίο με βαθιά φιλοσοφικά μηνύματα. Χρειάζεται προσεκτική ανάγνωση για να κατανοήσεις πλήρως το περιεχόμενο.",
                "Απογοητευτικό. Περίμενα πολύ περισσότερα από αυτόν τον συγγραφέα. Η πλοκή είναι αδύναμη και το τέλος βιαστικό.",
                "Εκπληκτικό! Ένα βιβλίο που θα μείνει στη μνήμη μου για πάντα. Η ιστορία είναι συναρπαστική και γεμάτη εκπλήξεις.",
                "Πολύ καλό βιβλίο για αρχάριους στο είδος. Η γλώσσα είναι απλή και κατανοητή, και η ιστορία ενδιαφέρουσα."
            };

            foreach (var book in books)
            {
                var reviewCount = random.Next(2, 5);
                var selectedUsers = users.OrderBy(x => random.Next()).Take(reviewCount).ToList();

                foreach (var user in selectedUsers)
                {
                    reviews.Add(new Review
                    {
                        Content = sampleReviews[random.Next(sampleReviews.Length)],
                        Rating = random.Next(2, 6), 
                        BookId = book.Id,
                        UserId = user.Id,
                        DateCreated = DateTime.UtcNow.AddDays(-random.Next(1, 30))
                    });
                }
            }

            return reviews;
        }

        private static List<ReviewVote> CreateReviewVotes(List<Review> reviews, List<User> users)
        {
            var votes = new List<ReviewVote>();
            var random = new Random();

            foreach (var review in reviews)
            {
                var availableUsers = users.Where(u => u.Id != review.UserId).ToList();
                var voteCount = random.Next(0, Math.Min(4, availableUsers.Count + 1));
                var votingUsers = availableUsers.OrderBy(x => random.Next()).Take(voteCount).ToList();

                foreach (var user in votingUsers)
                {
                    votes.Add(new ReviewVote
                    {
                        ReviewId = review.Id,
                        UserId = user.Id,
                        IsUpvote = random.NextDouble() > 0.3,
                        DateCreated = DateTime.UtcNow.AddDays(-random.Next(1, 25))
                    });
                }
            }

            return votes;
        }
    }
}