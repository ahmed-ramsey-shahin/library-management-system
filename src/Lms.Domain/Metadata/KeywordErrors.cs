using Lms.Domain.Common.Results;

namespace Lms.Domain.Metadata
{
    public static class KeywordErrors
    {
        public static Error IdRequired => Error.Validation("Keyword.IdRequired", "Keyword ID is required.");
        public static Error NameRequired => Error.Validation("Keyword.NameRequired", "Keyword name is required.");
        public static Error KeywordHasBooks => Error.Conflict("Keyword.HasBooks", "Cannot delete or modify the keyword because it is still associated with books.");
    }
}
