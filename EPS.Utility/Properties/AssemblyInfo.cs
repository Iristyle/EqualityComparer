using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("EPS.Utility")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyProduct("EPS.Utility")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("34a5780f-8fc9-49d4-90c4-67d866e3c693")]

[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Utility", Scope = "assembly", Justification = "EPS is a proper company name")]

[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Collections", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Configuration", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Configuration.Abstractions", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Conversions", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Diagnostics", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Dynamic", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.IO", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Linq", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Linq.Expressions", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Net", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Reflection", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Reflection.Emit", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Runtime.Serialization.Json", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Security", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Security.Cryptography", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Text", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Threading", Scope = "namespace", Justification = "EPS is a proper company name")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Target = "EPS.Windows.Security", Scope = "namespace", Justification = "EPS is a proper company name")]

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS.Collections", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS.Conversions", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS.Diagnostics", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS.Dynamic", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS.IO", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS.Linq", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS.Linq.Expressions", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS.Net", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS.Reflection", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS.Reflection.Emit", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS.Runtime.Serialization.Json", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS.Security", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS.Security.Cryptography", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS.Threading", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EPS.Windows.Security", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]