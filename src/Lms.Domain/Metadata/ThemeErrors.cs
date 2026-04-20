using Lms.Domain.Common.Results;

namespace Lms.Domain.Metadata
{
    public static class ThemeErrors
    {
        public static Error IdRequired => Error.Validation("Theme.Id.Required", "Theme id is required.");
        public static Error NameRequired => Error.Validation("Theme.Name.Required", "Theme name is required.");
        public static Error ThemeHasBooks => Error.Conflict("Theme.HasBooks", "This operation cannot be completed because this theme still has books associated with it.");
    }
}
