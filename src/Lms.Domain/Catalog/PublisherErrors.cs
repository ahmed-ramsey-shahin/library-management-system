using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public static class PublisherErrors
    {
        public static Error IdRequired => Error.Validation("Publisher.IdRequired", "Publisher ID is required.");
        public static Error NameRequired => Error.Validation("Publisher.NameRequired", "Publisher name is required.");
        public static Error PublisherHasBooks => Error.Conflict("Publisher.HasBooks", "The publisher cannot be deleted because they still have associated books in the catalog.");
    }
}
