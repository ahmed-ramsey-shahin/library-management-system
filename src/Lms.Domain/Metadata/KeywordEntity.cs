using Lms.Domain.Common.Results;

namespace Lms.Domain.Metadata
{
    public static class KeywordErrors
    {
        public static Error IdRequired => Error.Validation("Keyword.Id.Required", "Keyword id is required.");
        public static Error NameRequired => Error.Validation("Keyword.Name.Required", "Keyword name is required.");
        public static Error KeywordHasBooks => Error.Conflict("Keyword.HasBooks", "This operation cannot be completed because this keyword still has books associated with it.");
    }
}
