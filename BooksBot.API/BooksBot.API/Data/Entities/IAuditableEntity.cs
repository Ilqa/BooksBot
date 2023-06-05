using System;

namespace BooksBot.API.Data.Entities
{
    public interface IAuditableEntity
    {
        DateTime CreatedOn { get; set; }

        DateTime? ModifiedOn { get; set; }

    }
}
