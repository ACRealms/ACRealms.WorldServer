using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Diagnostics;

namespace ACRealms.MSBuild.Tasks
{
    // To Rebuild the project (visual studio)
    // You may get the error:
    // Could not copy "obj\Debug\netstandard2.0\ACRealms.MSBuild.dll" to "..\lib\ACRealms.MSBuild\Debug\netstandard2.0\ACRealms.MSBuild.dll".
    // Beginning retry 9 in 1000ms. The process cannot access the file '..\lib\ACRealms.MSBuild\Debug\netstandard2.0\ACRealms.MSBuild.dll' because it is being used by another process.
    // The file is locked by: "MSBuild.exe (25648)"
    //
    // Workaround
    // open command line and run the following:
    // set MSBUILDDISABLENODEREUSE=1
    // C:\Path\To\VisualStudio\devenv.exe
    public class CopyGeneratedJsonSchema : Task
    {

        public override bool Execute()
        {
            //if (!Debugger.IsAttached)
            //    Debugger.Launch();
            return true;
        }
    }
}
