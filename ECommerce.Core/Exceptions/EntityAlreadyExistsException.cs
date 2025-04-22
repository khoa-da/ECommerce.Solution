namespace ECommerce.Core.Exceptions
{
    public class EntityAlreadyExistsException : Exception
    {
        public EntityAlreadyExistsException() : base("Entity Already Exists") { }
        public EntityAlreadyExistsException(string message) : base(message) { }
        public EntityAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
        public EntityAlreadyExistsException(string entityName, object key)
        : base($"Entity already exists {entityName} with id {key}") { }
    }
}
