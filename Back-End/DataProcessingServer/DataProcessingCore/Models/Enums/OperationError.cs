namespace DataProcessingCore
{
    public enum OperationError
    {
        None = 0,
        EntityNotFound = 1,
        UnprocessableEntity = 2,
        ValidationError = 3,
        DatabaseError = 4,
        ExternalError = 5
    }
}
