namespace Lms.Domain.Catalog
{
    public sealed class BookTheme
    {
        public Guid BookId { get; }
        public Guid ThemeId { get; }
        private BookTheme()
        {}

        internal BookTheme(Guid bookId, Guid themeId)
        {
            BookId = bookId;
            ThemeId = themeId;
        }
    }
}
