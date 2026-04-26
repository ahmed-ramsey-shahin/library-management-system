using Lms.Domain.Metadata;

namespace Lms.Domain.Catalog
{
    public sealed class BookTheme
    {
        public Guid BookId { get; }
        public Guid ThemeId { get; }

        public Book Book { get; } = null!;
        public Theme Theme { get; } = null!;

        private BookTheme()
        {}

        internal BookTheme(Guid bookId, Guid themeId)
        {
            BookId = bookId;
            ThemeId = themeId;
        }
    }
}
