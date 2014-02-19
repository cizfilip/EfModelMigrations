using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Exceptions
{
    /// <summary>
    /// Represents errors that occur during creating Model Transformation classes
    /// </summary>
    [Serializable]
    public class ModelTransformationValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ModelTransformationValidationException class.
        /// </summary>
        public ModelTransformationValidationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ModelTransformationValidationException class.
        /// </summary>
        /// <param name="message"> The message that describes the error. </param>
        public ModelTransformationValidationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ModelTransformationValidationException class.
        /// </summary>
        /// <param name="message"> The message that describes the error. </param>
        /// <param name="innerException"> The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        public ModelTransformationValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ModelTransformationValidationException class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext" /> that contains contextual information about the source or destination.
        /// </param>
        protected ModelTransformationValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
