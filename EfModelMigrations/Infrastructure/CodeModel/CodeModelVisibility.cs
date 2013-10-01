using System;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    [Flags]
    public enum CodeModelVisibility
    {
        Public = 0,
        Private = 1,
        Protected = 2,
        Internal = 4,
        ProtectedInternal = Protected | Internal
    }
}
