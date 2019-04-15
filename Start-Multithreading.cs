using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace TMK_BinaryModule
{
    [Cmdlet(VerbsLifecycle.Start, "MT")]
    public class Start_Multithreading : PSCmdlet
    {
        // The object(s) on which to execute the ScriptBlock
        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            Position = 0)]
        [Alias("InputObjects")]
        public ArrayList InputObject { get; set; }

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
            int c = 0;
            int count = InputObject.Count;

            if (MaxThreads < 20)
            {
                MaxThreads = 20;
            }
            using (RunspacePool runspacePool = RunspaceFactory.CreateRunspacePool(1, MaxThreads))
            {
                try
                {
                    runspacePool.Open();
                    ProgressRecord progressRecord = new ProgressRecord(1, "Action in progress...", "Processing...");

                    foreach (object obj in InputObject)
                    {
                        PowerShell powerShell = PowerShell.Create()
                            .AddScript(ScriptBlock)
                            .AddArgument(obj);

                        try
                        {
                            powerShell.AddParameters(ArgumentList);
                        }
                        catch (Exception)
                        {
                        }
                        powerShell.RunspacePool = runspacePool;

                        IAsyncResult psAsyncResult = powerShell.BeginInvoke();
                        c++;

                        int pVal = (c / count) * 100;
                        string sVal = String.Format("{0:N0}", pVal);
                        int perc = int.Parse(sVal);

                        string activity = c + " of " + count + " threads completed";
                        if (NoProgress.IsPresent == false)
                        {
                            progressRecord.PercentComplete = perc;
                            progressRecord.Activity = activity;
                            progressRecord.StatusDescription = perc + "% complete";
                            WriteProgress(progressRecord);
                        }

                        PSDataCollection<PSObject> psOutput = powerShell.EndInvoke(psAsyncResult);
                        final.Add(psOutput);
                        powerShell.Dispose();
                    } // End foreach

                    if (NoProgress.IsPresent == false)
                    {
                        progressRecord.PercentComplete = 100;
                        progressRecord.StatusDescription = "100% complete";
                        WriteProgress(progressRecord);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                runspacePool.Close();
                runspacePool.Dispose();
            } // End using

            // Output to console
            WriteObject(final.ToArray(), true);
        } // End begin
    }
}
