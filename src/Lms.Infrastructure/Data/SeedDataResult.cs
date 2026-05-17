using Lms.Domain.Catalog;
using Lms.Domain.Identity;
using Lms.Domain.Metadata;

namespace Lms.Infrastructure.Data
{
    public record SeedDataResult
    (
        List<Publisher> Publishers,
        List<Audience> Audiences,
        List<Author> Authors,
        List<Category> Categories,
        List<Genre> Genres,
        List<Keyword> Keywords,
        List<Theme> Themes,
        List<User> Users,
        List<Book> Books
    );
}
