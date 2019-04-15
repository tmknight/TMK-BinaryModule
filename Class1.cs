using System;
using System.Collections;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace TMK_BinaryModule
{
    [Cmdlet(VerbsLifecycle.Start, "MTPr")]
    public class Start_MultithreadingPr : PSCmdlet
    {
        // The object(s) on which to execute the ScriptBlock
        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            Position = 0)]
        [Alias("InputObjects")]
        public Array InputObject { get; set; }

        // Command or script to run
        // Must have a parameter of an object from inputobjects
        [Parameter(Mandatory = true,
            Position = 1)]
        public string ScriptBlock { get; set; }

        // List of arguments required by the scriptblock
        [Parameter(Mandatory = false,
            Position = 2)]
        [Alias("Arguments")]
        public Array ArgumentList { get; set; }

        // Maximum concurrent threads to start
        [Parameter(Mandatory = false,
            Position = 3)]
        public int MaxThreads { get; set; } = 20;

        // Whether progress is displayed
        [Parameter(Position = 4)]
        public SwitchParameter NoProgress;

        protected override void BeginProcessing()
        {
                // Build the results
                ArrayList final = new ArrayList();
                Hashtable rsColl = new Hashtable();

                int c = 0;
                using (RunspacePool runspacePool = RunspaceFactory.CreateRunspacePool(1, MaxThreads))
                {
                    try
                    {
                        runspacePool.Open();
                        foreach (object obj in InputObject)
                        {
                            PowerShell powerShell = PowerShell.Create();
                            powerShell
                                .AddScript(ScriptBlock)
                                .AddArgument(obj);

                            try
                            {
                                powerShell.AddParameters(ArgumentList);
                            }
                            catch (Exception)
                            {
                            }

                            IAsyncResult psAsyncResult = powerShell.BeginInvoke();

                            rsColl.Add("psResult", psAsyncResult);
                            rsColl.Add("psPowerShell", powerShell);

                            //PSDataCollection<PSObject> psOutput = powerShell.EndInvoke(psAsyncResult);
                            //final.Add(psOutput);
                            //powerShell.Dispose();
                        } // End foreach
                        //runspacePool.Close();
                        //runspacePool.Dispose();
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                } // End using

                // Output to console
                WriteObject(final, true);

            }
        } // End begin
}
