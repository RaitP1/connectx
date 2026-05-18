using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ConnectX.UnitTests")]

namespace Application;

public static class ApplicationMarker
{
    internal static readonly Type DomainRef = typeof(Domain.DomainMarker);
}
