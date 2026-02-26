using Xunit;
using Moq;
using ProcessM.Core.Services;
using ProcessM.Core.Models;
using System.Collections.Generic;
using System.Diagnostics;

public class ProcessServiceTests
{
    [Fact]
    public void KillProcess_ShouldNotThrow()
    {
        var service = new ProcessService();

        service.KillProcess(0); // несуществующий

        Assert.True(true);
    }

    [Fact]
    public void GetProcesses_ShouldReturnList()
    {
        var service = new ProcessService();

        var list = service.GetProcesses();

        Assert.NotNull(list);
    }
}