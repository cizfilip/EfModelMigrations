using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Exceptions
{
 
    /// <summary>
    /// Represents errors that occur inside the Ef Model Migrations pipeline.
    /// </summary>
    [Serializable]
    public class ModelMigrationsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ModelMigrationException class.
        /// </summary>
        public ModelMigrationsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ModelMigrationException class.
        /// </summary>
        /// <param name="message"> The message that describes the error. </param>
        public ModelMigrationsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ModelMigrationException class.
        /// </summary>
        /// <param name="message"> The message that describes the error. </param>
        /// <param name="innerException"> The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        public ModelMigrationsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ModelMigrationException class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext" /> that contains contextual information about the source or destination.
        /// </param>
        protected ModelMigrationsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
