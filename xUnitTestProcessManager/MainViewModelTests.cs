using Xunit;
using Moq;
using ProcessM.Core.ViewModels;
using ProcessM.Core.Services;
using ProcessM.Core.Models;
using System.Collections.Generic;
using System.Diagnostics;

public class MainViewModelTests
{
    [Fact]
    public void KillSelected_ShouldCallService()
    {
        var mock = new Mock<IProcessService>();
        var vm = new MainViewModel(mock.Object);

        vm.SelectedProcess = new ProcessInfo { Id = 123 };

        vm.KillSelected();

        mock.Verify(s => s.KillProcess(123), Times.Once);
    }

    [Fact]
    public void ChangePriority_ShouldCallService()
    {
        var mock = new Mock<IProcessService>();
        var vm = new MainViewModel(mock.Object);

        vm.SelectedProcess = new ProcessInfo { Id = 55 };

        vm.ChangePriority(ProcessPriorityClass.High);

        mock.Verify(s => s.SetPriority(55, ProcessPriorityClass.High), Times.Once);
    }

    [Fact]
    public void SyncCollection_ShouldAddProcesses()
    {
        var mock = new Mock<IProcessService>();
        var vm = new MainViewModel(mock.Object);

        var list = new List<ProcessInfo>
        {
            new ProcessInfo { Id = 1, Name = "Test", MemoryUsage = 100 }
        };

        vm.SyncCollection(list);

        Assert.Single(vm.Processes);
    }
}