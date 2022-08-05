using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ALMA_API.Models.Requests
{
    public class BaseRequest
    {
        public T Merge<T>(EntityEntry<T> entry) where T : class
        {
            var entity = entry.Entity;
            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (propertyInfo.GetValue(this) is {} value)
                {
                    entry.Property(propertyInfo.Name).CurrentValue = value;
                }
            }

            return entity;
        }
    }
}
