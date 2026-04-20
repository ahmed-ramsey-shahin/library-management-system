using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public static class PublisherErrors
    {
        public static Error IdRequired => Error.Validation("Publisher.Id.Required", "Publisher id is required.");
        public static Error NameRequired => Error.Validation("Publisher.Name.Required", "Publisher name is required.");
        public static Error PublisherHasBooks => Error.Conflict("Publisher.HasBooks", "This operation cannot be completed because the publisher still has books associated with it.");
    }
}
