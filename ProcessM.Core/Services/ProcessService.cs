using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ProcessM.Core.Models;

namespace ProcessM.Core.Services
{
    public class ProcessService : IProcessService
    {
        public List<ProcessInfo> GetProcesses()
        {
            var list = new List<ProcessInfo>();

            foreach (var p in Process.GetProcesses())
            {
                try
                {
                    list.Add(new ProcessInfo
                    {
                        Id = p.Id,
                        Name = p.ProcessName,
                        Priority = p.PriorityClass,
                        MemoryUsage = p.WorkingSet64 / 1024 / 1024,
                        ThreadCount = p.Threads.Count,
                        CpuTime = p.TotalProcessorTime,
                        AffinityMask = p.ProcessorAffinity,
                        Threads = p.Threads.Cast<ProcessThread>().Select(t => new ThreadInfo
                        {
                            Id = t.Id,
                            Priority = t.PriorityLevel,
                            State = t.ThreadState,
                            CpuTime = t.TotalProcessorTime
                        }).ToList()
                    });
                }
                catch
                {
                    // нет доступа к процессу
                }
            }

            return list;
        }

        public void KillProcess(int id)
        {
            try { Process.GetProcessById(id).Kill(); }
            catch { }
        }

        public void SetPriority(int id, ProcessPriorityClass priority)
        {
            try { Process.GetProcessById(id).PriorityClass = priority; }
            catch { }
        }
    }
}