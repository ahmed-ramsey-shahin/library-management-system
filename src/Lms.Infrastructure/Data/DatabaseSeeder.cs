using Bogus;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Catalog;
using Lms.Domain.Identity;
using Lms.Domain.Metadata;

namespace Lms.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        private static int IndexGlobal = 1;

        public static SeedDataResult GenerateAllMockData(
            IPasswordHasher passwordHasher,
            int userCount = 25,
            int publisherCount = 8,
            int metadataCount = 12,
            int bookCount = 30
        )
        {
            var faker = new Faker();

            // PHASE 1: Publishers & Metadata
            List<Publisher> publishers = [];

            for (int i = 0; i < publisherCount; i++)
            {
                var publisherResult = Publisher.Create(
                    Guid.NewGuid(),
                    $"{faker.Company.CompanyName()} #{IndexGlobal++}"
                );
                if (publisherResult.IsSuccess) publishers.Add(publisherResult.Value);
            }

            List<Audience> audiences = [];
            List<Author> authors = [];
            List<Category> categories = [];
            List<Genre> genres = [];
            List<Keyword> keywords = [];
            List<Theme> themes = [];

            for (int i = 0; i < metadataCount; i++)
            {
                var aud = Audience.Create(Guid.NewGuid(), $"{faker.Commerce.Department()} Section {IndexGlobal++}");
                if (aud.IsSuccess) audiences.Add(aud.Value);

                var aut = Author.Create(Guid.NewGuid(), $"{faker.Name.FullName()} [{IndexGlobal++}]");
                if (aut.IsSuccess) authors.Add(aut.Value);

                var cat = Category.Create(Guid.NewGuid(), $"{faker.Commerce.Categories(1)[0]} Dept {IndexGlobal++}");
                if (cat.IsSuccess) categories.Add(cat.Value);

                var gen = Genre.Create(Guid.NewGuid(), $"{faker.Music.Genre()} Style {IndexGlobal++}");
                if (gen.IsSuccess) genres.Add(gen.Value);

                var key = Keyword.Create(Guid.NewGuid(), $"{faker.Random.Word().ToLower()}{IndexGlobal++}");
                if (key.IsSuccess) keywords.Add(key.Value);

                var thm = Theme.Create(Guid.NewGuid(), $"{faker.Random.Words(2)} Concept {IndexGlobal++}");
                if (thm.IsSuccess) themes.Add(thm.Value);
            }

            // PHASE 2: System users
            List<User> users = [];

            for (int i = 0; i < userCount; i++)
            {
                var role = i == 0 ? Role.Admin : (i <= 3 ? Role.Librarian : Role.Member);
                var secureHash = passwordHasher.Hash("Lms2026@!");
                var user = User.Create(
                    Guid.NewGuid(),
                    $"{faker.Internet.UserName().ToLower()}{IndexGlobal++}@library.com",
                    faker.Name.FirstName(),
                    faker.Name.LastName(),
                    $"+1{IndexGlobal++:D9}",
                    faker.Address.FullAddress(),
                    User.GenerateLibraryNumber(),
                    secureHash,
                    role,
                    UserStatus.Active
                );

                if (user.IsSuccess) users.Add(user.Value);
                if (role == Role.Librarian && categories.Count > 0)
                {
                    var pickedCategories = faker.PickRandom(categories, faker.Random.Number(1, 3));
                    user.Value.UpsertCategories(pickedCategories.Select(category => category.Id));
                }
            }

            // PHASE 3: Books
            List<Book> books = [];

            if (publishers.Count > 0)
            {
                for (int i = 0; i < bookCount; i++)
                {
                    var publisher = faker.PickRandom(publishers);
                    var isbn = $"978{IndexGlobal++:D10}";
                    var issn = $"2{IndexGlobal++:D7}";
                    var book = Book.Create(
                        Guid.NewGuid(),
                        isbn,
                        issn,
                        $"{faker.Commerce.ProductName()} Vol. {IndexGlobal++}",
                        faker.Lorem.Paragraph(),
                        faker.Random.Number(120, 950),
                        publisher.Id,
                        DateOnly.FromDateTime(faker.Date.Past(4)),
                        $"{faker.Random.Number(1, 3)}nd edition",
                        faker.Finance.Amount(1, 4),
                        faker.Finance.Amount(2, 5),
                        faker.Finance.Amount(40, 90),
                        faker.Finance.Amount(15, 40),
                        "English"
                    );

                    if (book.IsSuccess)
                    {
                        if (audiences.Count > 0) book.Value.UpsertAudiences(faker.PickRandom(audiences, 2).Select(audience => audience.Id));
                        if (authors.Count > 0) book.Value.UpsertAuthors(faker.PickRandom(authors, 2).Select(author => author.Id));
                        if (categories.Count > 0) book.Value.UpsertCategories(faker.PickRandom(categories, 2).Select(category => category.Id));
                        if (keywords.Count > 0) book.Value.UpsertKeywords(faker.PickRandom(keywords, 2).Select(keyword => keyword.Id));
                        if (genres.Count > 0) book.Value.UpsertGenres(faker.PickRandom(genres, 2).Select(genre => genre.Id));
                        if (themes.Count > 0) book.Value.UpsertThemes(faker.PickRandom(themes, 2).Select(theme => theme.Id));

                        int stockQuantity = faker.Random.Number(1, 5);

                        for (int j = 0; j < stockQuantity; j++)
                        {
                            book.Value.AddCopy(
                                Guid.NewGuid(),
                                $"CPY-{IndexGlobal++:D8}",
                                $"Shelf {faker.Random.Char('A', 'Z')}-{faker.Random.Number(1, 10):D2}",
                                DateOnly.FromDateTime(faker.Date.Past(2))
                            );
                        }

                        books.Add(book.Value);
                    }
                }
            }

            return new SeedDataResult(
                publishers,
                audiences,
                authors,
                categories,
                genres,
                keywords,
                themes,
                users,
                books
            );
        }
    }
}
