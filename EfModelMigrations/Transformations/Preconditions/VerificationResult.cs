namespace EfModelMigrations.Transformations.Preconditions
{
    public sealed class VerificationResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        private VerificationResult(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
        }

        public static VerificationResult Successful(string message = null)
        {
            return new VerificationResult(true, message);
        }

        public static VerificationResult Error(string message)
        {
            Check.NotEmpty(message, "message");

            return new VerificationResult(false, message);
        }
    }
}
