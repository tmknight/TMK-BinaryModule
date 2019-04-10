using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace TMK_BinaryModule
{
    [Cmdlet(VerbsCommon.Get, "SampleCmdlet")]
        // VerbsDiagnostic.Test, "SampleCmdlet")]
    [OutputType(typeof(Results))]
    public class TestSampleCmdletCommand : PSCmdlet
    {
        [Parameter(
            Position = 1,
            ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; } //= "Dog";

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin!");
            PowerShell ps = PowerShell.Create()
                .AddCommand("Get-Process")
                .AddCommand("Where-Object")
                    .AddParameter("Property", "Name")
                    .AddParameter("Value", Name)
                    .AddParameter("Match");

            IAsyncResult async = ps.BeginInvoke();

            foreach (PSObject result in ps.EndInvoke(async))
            {
                //Console.WriteLine("{0,-20}{1}",
                //        result.Members["ProcessName"].Value,
                //        result.Members["Id"].Value);
                var procName = result.Members["ProcessName"].Value.ToString();
                var procId = result.Members["Id"].Value.ToString();
                WriteObject(new Results
                    {
                        Name = procName,
                        PID = procId
                    }
                );
            } // End foreach.
            //System.Console.WriteLine("Hit any key to exit.");
            //System.Console.ReadKey();
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        //protected override void ProcessRecord()
        //{
        //                WriteObject(new Results
        //                {
        //                    Name = procName,
        //                    ID = procID
        //                });
        //
        //}

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        //protected override void EndProcessing()
        //{
        //    WriteVerbose("End!");
        //}
    }

    public class Results
    {
        public string Name { get; set; }
        public string PID { get; set; }
    }
}
