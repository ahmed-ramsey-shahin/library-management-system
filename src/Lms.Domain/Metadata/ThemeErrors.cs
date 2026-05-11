using Lms.Domain.Common.Results;

namespace Lms.Domain.Metadata
{
    public static class ThemeErrors
    {
        public static Error IdRequired => Error.Validation("Theme.IdRequired", "Theme ID is required.");
        public static Error NameRequired => Error.Validation("Theme.NameRequired", "Theme name is required.");
        public static Error ThemeHasBooks => Error.Conflict("Theme.HasBooks", "Cannot delete or modify the theme because it is still associated with books.");
    }
}
