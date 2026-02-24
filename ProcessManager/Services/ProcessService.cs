using System.Collections.Generic;
using System.Diagnostics;
using ProcessManager.Models;

namespace ProcessManager.Services
{
    public static class ProcessService
    {
        public static List<ProcessTreeNode> GetProcesses()
        {
            var list = new List<ProcessTreeNode>();

            foreach (var p in Process.GetProcesses())
            {
                try
                {
                    list.Add(new ProcessTreeNode
                    {
                        Id = p.Id,
                        Name = p.ProcessName,
                        Memory = p.WorkingSet64 / 1024 / 1024
                    });
                }
                catch { }
            }

            return list;
        }

        public static void KillProcess(int id)
        {
            try
            {
                Process.GetProcessById(id).Kill();
            }
            catch { }
        }

        public static void SetPriority(int id, ProcessPriorityClass priority)
        {
            try
            {
                Process.GetProcessById(id).PriorityClass = priority;
            }
            catch { }
        }
    }
}