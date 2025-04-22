namespace ECommerce.Core.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException() : base("No data found") { }
        public EntityNotFoundException(string message) : base(message) { }
        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) { }
        public EntityNotFoundException(string entityName, object key)
            : base($"No data found {entityName} with id {key}") { }
    }
}
