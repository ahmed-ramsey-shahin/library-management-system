using Lms.Domain.Common.Results;

namespace Lms.Domain.Metadata
{
    public static class AudienceErrors
    {
        public static Error IdRequired => Error.Validation("Audience.IdRequired", "Audience ID is required.");
        public static Error NameRequired => Error.Validation("Audience.NameRequired", "Audience name is required.");
        public static Error AudienceHasBooks => Error.Conflict("Audience.HasBooks", "Cannot delete or modify the audience because it is still associated with books.");
    }
}
