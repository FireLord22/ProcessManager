using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
                        MemoryUsage = p.WorkingSet64 / 1024 / 1024,
                        ThreadCount = p.Threads.Count,
                        CpuTime = p.TotalProcessorTime,
                        Priority = SafePriority(p),
                        AffinityMask = SafeAffinity(p)
                    });
                }
                catch (Exception ex) when (
    ex is Win32Exception ||
    ex is InvalidOperationException)
                {
                    list.Add(new ProcessInfo
                    {
                        Id = p.Id,
                        Name = p.ProcessName,
                        
                    });
                }
            }

            return list;
        }

        public void KillProcess(int id)
        {
            try
            {
                Process.GetProcessById(id).Kill();
            }
            catch
            {
            }
        }

        public void SetPriority(int id, ProcessPriorityClass priority)
        {
            try
            {
                Process.GetProcessById(id).PriorityClass = priority;
            }
            catch
            {
            }
        }

        public IntPtr GetAffinity(int id)
        {
            try
            {
                return Process.GetProcessById(id).ProcessorAffinity;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        public void SetAffinity(int id, IntPtr mask)
        {
            try
            {
                Process.GetProcessById(id).ProcessorAffinity = mask;
            }
            catch
            {
            }
        }

        private ProcessPriorityClass SafePriority(Process p)
        {
            try
            {
                return p.PriorityClass;
            }
            catch
            {
                return ProcessPriorityClass.Normal;
            }
        }

        private IntPtr SafeAffinity(Process p)
        {
            try
            {
                return p.ProcessorAffinity;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        public List<ThreadInfo> GetThreads(int processId)
        {
            var list = new List<ThreadInfo>();

            try
            {
                var p = Process.GetProcessById(processId);

                foreach (ProcessThread t in p.Threads)
                {
                    list.Add(new ThreadInfo
                    {
                        Id = t.Id,
                        Priority = t.PriorityLevel,
                        State = t.ThreadState,
                        CpuTime = t.TotalProcessorTime
                    });
                }
            }
            catch
            {
            }

            return list;
        }
    }
}