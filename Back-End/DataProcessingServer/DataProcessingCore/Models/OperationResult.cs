namespace DataProcessingCore
{
    public class OperationResult<TOutput> where TOutput : class
    {

        #region Properties


        public TOutput? Output { get; set; }


        public IDictionary<string, string> Errors { get; set; }

        public OperationError ErrorType { get; set; }
        public bool IsSuccessful => ErrorType == OperationError.None;

        #endregion


        #region Constructors

        public OperationResult()
        {
            Errors = new Dictionary<string, string>();
            ErrorType = OperationError.None;
        }

        public OperationResult(TOutput? output) : this()
        {
            Output = output;
        }

        public OperationResult(OperationError errorType, string message = "") : this()
        {
            ErrorType = errorType;
            Errors.Add(errorType.ToString(), message);
        }

        public OperationResult(IDictionary<string, string> errors, OperationError errorType)
        {
            Errors = errors;
            ErrorType = errorType;
        }

        #endregion

    }

}
