using Lms.Domain.Common.Results;

namespace Lms.Domain.Metadata
{
    public static class AudienceErrors
    {
        public static Error IdRequired => Error.Validation("Audience.Id.Required", "Audience id is required.");
        public static Error NameRequired => Error.Validation("Audience.Name.Required", "Audience name is required.");
        public static Error AudienceHasBooks => Error.Conflict("Audience.HasBooks", "This operation cannot be completed because this type of audience still has books associated with it.");
    }
}
