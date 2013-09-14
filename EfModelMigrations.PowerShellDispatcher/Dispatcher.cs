using EnvDTE;
using System;
using System.Management.Automation;

namespace EfModelMigrations.PowerShellDispatcher
{
    /// <summary>
    ///     Provides a way of dispatching specific calls form the PowerShell commands'
    ///     AppDomain to the Visual Studio's main AppDomain.
    /// </summary>

    [CLSCompliant(false)]
    public class Dispatcher : MarshalByRefObject
    {
        private readonly PSCmdlet cmdlet;
        private readonly DTE dte;

        public Dispatcher()
        {
            // Testing    
        }

        public Dispatcher(PSCmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }

            this.cmdlet = cmdlet;
            this.dte = (DTE)cmdlet.GetVariableValue("DTE");
        }

        public void WriteLine(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException("text");
            }

            cmdlet.Host.UI.WriteLine(text);
        }

        public void WriteWarning(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException("text");
            }

            cmdlet.WriteWarning(text);
        }

        public void WriteVerbose(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException("text");
            }

            cmdlet.WriteVerbose(text);
        }

        public virtual void OpenFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            dte.ItemOperations.OpenFile(fileName);
        }
    }

}
