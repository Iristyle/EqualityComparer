using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("EqualityComparer")]
[assembly: AssemblyDescription("A set of Expression tree based object instance comparers")]
[assembly: AssemblyProduct("EqualityComparer")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyCompany("East Point Systems, Inc. http://www.eastpointsystems.com/")]
[assembly: AssemblyCopyright("Copyright © 2012 East Point Systems, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyVersion("0.1.2.0")]
[assembly: AssemblyFileVersion("0.1.2.0")]
[assembly: AssemblyInformationalVersion("0.1.2.0")]
[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguage("en")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("34a5780f-8fc9-49d4-90c4-67d866e3c693")]

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EqualityComparer", Scope = "namespace", Justification = "Simple library!")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Target = "EqualityComparer.Reflection", Scope = "namespace", Justification = "Helpers mirror .NET framework type layout")]
