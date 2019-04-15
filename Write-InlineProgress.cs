using System;
using System.Collections;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace TMK_BinaryModule
{
    [Cmdlet(VerbsCommunications.Write, "InlineProgress")]
    public class Write_InlineProgress : PSCmdlet
    {
        // The object(s) on which to execute the ScriptBlock
        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0)]
        public string Activity { get; set; }

        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 1)]
        public int PercentComplete { get; set; }

        protected override void BeginProcessing()
        {
            int perc;
            Int32.TryParse(PercentComplete.ToString("F",
                CultureInfo.InvariantCulture), out perc);

            switch (Host.Name)
            {
                case "Visual Studio Code Host":
                    Version pSHost = Host.Version;
                    if (pSHost.Major >= 2 && pSHost.Minor >= 0 && pSHost.Build >= 1)
                    {
                        WriteProgress(new ProgressRecord(perc, Activity, "Processing..."));
                    }
                    else
                    {
                        string val = Activity + " " + perc + "%    ";
                        int cursorY = Host.UI.RawUI.CursorPosition.Y;
                        Console.SetCursorPosition(0, cursorY);
                        Console.Write(val);
                    }

                    break;
                default:
                    WriteProgress(new ProgressRecord(perc, Activity, ""));
                    break;
            }

            // Output to console
            // WriteObject(final, true);
        } // End begin
    }
}
