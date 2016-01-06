using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fay.Logging;
using Fay.Logging.NLog4;
using Moq;
using NLog;
using Ploeh.AutoFixture.Xunit2;
using Shouldly;
using Xunit;

namespace Logging.NLog4.UnitTests
{
    public class NLogSimpleLoggerUnitTests
    {
        [Theory]
        [InlineAutoData("Critical", LogSeverity.Critical, NLogLevel.Fatal, true, null)]
        [InlineAutoData("Critical", LogSeverity.Critical, NLogLevel.Error, true, null)]
        [InlineAutoData("Critical", LogSeverity.Critical, NLogLevel.Warn, true, null)]
        [InlineAutoData("Critical", LogSeverity.Critical, NLogLevel.Info, true, null)]
        [InlineAutoData("Critical", LogSeverity.Critical, NLogLevel.Debug, true, null)]
        [InlineAutoData("Critical", LogSeverity.Critical, NLogLevel.Trace, true, null)]
        [InlineAutoData("Critical", LogSeverity.Critical, NLogLevel.Off, false, null)]
        [InlineAutoData("Error", LogSeverity.Error, NLogLevel.Fatal, false, null)]
        [InlineAutoData("Error", LogSeverity.Error, NLogLevel.Error, true, null)]
        [InlineAutoData("Error", LogSeverity.Error, NLogLevel.Warn, true, null)]
        [InlineAutoData("Error", LogSeverity.Error, NLogLevel.Info, true, null)]
        [InlineAutoData("Error", LogSeverity.Error, NLogLevel.Debug, true, null)]
        [InlineAutoData("Error", LogSeverity.Error, NLogLevel.Trace, true, null)]
        [InlineAutoData("Error", LogSeverity.Error, NLogLevel.Off, false, null)]
        [InlineAutoData("Warning", LogSeverity.Warning, NLogLevel.Fatal, false, null)]
        [InlineAutoData("Warning", LogSeverity.Warning, NLogLevel.Error, false, null)]
        [InlineAutoData("Warning", LogSeverity.Warning, NLogLevel.Warn, true, null)]
        [InlineAutoData("Warning", LogSeverity.Warning, NLogLevel.Info, true, null)]
        [InlineAutoData("Warning", LogSeverity.Warning, NLogLevel.Debug, true, null)]
        [InlineAutoData("Warning", LogSeverity.Warning, NLogLevel.Trace, true, null)]
        [InlineAutoData("Warning", LogSeverity.Warning, NLogLevel.Off, false, null)]
        [InlineAutoData("Information", LogSeverity.Information, NLogLevel.Fatal, false, null)]
        [InlineAutoData("Information", LogSeverity.Information, NLogLevel.Error, false, null)]
        [InlineAutoData("Information", LogSeverity.Information, NLogLevel.Warn, false, null)]
        [InlineAutoData("Information", LogSeverity.Information, NLogLevel.Info, true, null)]
        [InlineAutoData("Information", LogSeverity.Information, NLogLevel.Debug, true, null)]
        [InlineAutoData("Information", LogSeverity.Information, NLogLevel.Trace, true, null)]
        [InlineAutoData("Information", LogSeverity.Information, NLogLevel.Off, false, null)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, NLogLevel.Fatal, false, null)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, NLogLevel.Error, false, null)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, NLogLevel.Warn, false, null)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, NLogLevel.Info, false, null)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, NLogLevel.Debug, true, null)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, NLogLevel.Trace, true, null)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, NLogLevel.Off, false, null)]
        [InlineAutoData("CriticalException", LogSeverity.Critical, NLogLevel.Fatal, true)]
        [InlineAutoData("CriticalException", LogSeverity.Critical, NLogLevel.Fatal, false, null, null)]
        [InlineAutoData("CriticalException", LogSeverity.Critical, NLogLevel.Off, false)]
        [InlineAutoData("ErrorException", LogSeverity.Error, NLogLevel.Error, true)]
        [InlineAutoData("ErrorException", LogSeverity.Error, NLogLevel.Fatal, false)]
        [InlineAutoData("ErrorException", LogSeverity.Error, NLogLevel.Error, false, null, null)]
        [InlineAutoData("ErrorException", LogSeverity.Error, NLogLevel.Error, true)]
        [InlineAutoData("ErrorException", LogSeverity.Error, NLogLevel.Off, false)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, NLogLevel.Debug, false, null, null)]
        [InlineAutoData("ErrorException", LogSeverity.Error, NLogLevel.Debug, true, null)]
        public void LoggerLogProvidedExpectedDataIfInScopeAndDataIsValid(string methodName, LogSeverity expectedLogSeverity, NLogLevel thresholdLevel, bool isExpectedToBeLogged, Exception expectedException, string expectedMessage)
        {
            // Arrange
            Mock<ILogger> iLoggerMock = CreateILoggerMock(thresholdLevel);
            IDelegateLogger<string> sut = new NLogSimpleLogger(iLoggerMock.Object);
            Times times = isExpectedToBeLogged ? Times.Once() : Times.Never();
            Func<string> writeLogEntry = () => expectedMessage;

            // Act
            if (methodName.EndsWith("Exception"))
                sut.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, sut, new object[] { writeLogEntry, expectedException });
            else
                sut.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, sut, new object[] { writeLogEntry });

            // Assert
            iLoggerMock.Verify(l => l.Log(_logSeverityToLogLevelMap[expectedLogSeverity], expectedException, expectedMessage), times);
        }

        [Theory]
        [InlineData(NLogLevel.Fatal, LogSeverity.Critical, true)]
        [InlineData(NLogLevel.Fatal, LogSeverity.Error, false)]
        [InlineData(NLogLevel.Fatal, LogSeverity.Warning, false)]
        [InlineData(NLogLevel.Fatal, LogSeverity.Information, false)]
        [InlineData(NLogLevel.Fatal, LogSeverity.Verbose, false)]
        [InlineData(NLogLevel.Error, LogSeverity.Critical, true)]
        [InlineData(NLogLevel.Error, LogSeverity.Error, true)]
        [InlineData(NLogLevel.Error, LogSeverity.Warning, false)]
        [InlineData(NLogLevel.Error, LogSeverity.Information, false)]
        [InlineData(NLogLevel.Error, LogSeverity.Verbose, false)]
        [InlineData(NLogLevel.Warn, LogSeverity.Critical, true)]
        [InlineData(NLogLevel.Warn, LogSeverity.Error, true)]
        [InlineData(NLogLevel.Warn, LogSeverity.Warning, true)]
        [InlineData(NLogLevel.Warn, LogSeverity.Information, false)]
        [InlineData(NLogLevel.Warn, LogSeverity.Verbose, false)]
        [InlineData(NLogLevel.Info, LogSeverity.Critical, true)]
        [InlineData(NLogLevel.Info, LogSeverity.Error, true)]
        [InlineData(NLogLevel.Info, LogSeverity.Warning, true)]
        [InlineData(NLogLevel.Info, LogSeverity.Information, true)]
        [InlineData(NLogLevel.Info, LogSeverity.Verbose, false)]
        [InlineData(NLogLevel.Debug, LogSeverity.Critical, true)]
        [InlineData(NLogLevel.Debug, LogSeverity.Error, true)]
        [InlineData(NLogLevel.Debug, LogSeverity.Warning, true)]
        [InlineData(NLogLevel.Debug, LogSeverity.Information, true)]
        [InlineData(NLogLevel.Debug, LogSeverity.Verbose, true)]
        [InlineData(NLogLevel.Trace, LogSeverity.Critical, true)]
        [InlineData(NLogLevel.Trace, LogSeverity.Error, true)]
        [InlineData(NLogLevel.Trace, LogSeverity.Warning, true)]
        [InlineData(NLogLevel.Trace, LogSeverity.Information, true)]
        [InlineData(NLogLevel.Trace, LogSeverity.Verbose, true)]
        [InlineData(NLogLevel.Off, LogSeverity.Critical, false)]
        [InlineData(NLogLevel.Off, LogSeverity.Error, false)]
        [InlineData(NLogLevel.Off, LogSeverity.Warning, false)]
        [InlineData(NLogLevel.Off, LogSeverity.Information, false)]
        [InlineData(NLogLevel.Off, LogSeverity.Verbose, false)]
        public void IsSeverityInScopeReturnsValid(NLogLevel thresholdLevel, LogSeverity logSeverity, bool expected)
        {
            // Arrange
            Mock<ILogger> iLoggerMock = CreateILoggerMock(thresholdLevel);
            IDelegateLogger<string> sut = new NLogSimpleLogger(iLoggerMock.Object);

            // Act
            bool result = sut.IsSeverityInScope(logSeverity, null);

            // Assert
            result.ShouldBe(expected);
        }

        public enum NLogLevel
        {
            Off,
            Fatal,
            Error,
            Warn,
            Info,
            Debug,
            Trace,
        }

        private readonly IDictionary<NLogLevel, LogLevel> _logLevelToLogLevelMap = new Dictionary<NLogLevel, LogLevel>
        {
            {NLogLevel.Off, LogLevel.Off},
            {NLogLevel.Fatal, LogLevel.Fatal},
            {NLogLevel.Error, LogLevel.Error},
            {NLogLevel.Warn, LogLevel.Warn},
            {NLogLevel.Info, LogLevel.Info},
            {NLogLevel.Debug, LogLevel.Debug},
            {NLogLevel.Trace, LogLevel.Trace},
        };

        private readonly IDictionary<LogSeverity, LogLevel> _logSeverityToLogLevelMap = new Dictionary<LogSeverity, LogLevel>
        {
            {LogSeverity.Off, LogLevel.Off},
            {LogSeverity.Critical, LogLevel.Fatal},
            {LogSeverity.Error, LogLevel.Error},
            {LogSeverity.Warning, LogLevel.Warn},
            {LogSeverity.Information, LogLevel.Info},
            {LogSeverity.Verbose, LogLevel.Debug},
            {LogSeverity.All, LogLevel.Trace},
        };

        private Mock<ILogger> CreateILoggerMock(NLogLevel thresholdLevel)
        {
            Mock<ILogger> iLoggerMock = new Mock<ILogger>();
            iLoggerMock.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns((LogLevel level) => IsEnabledStub(level, _logLevelToLogLevelMap[thresholdLevel]));
            return iLoggerMock;
        }

        private bool IsEnabledStub(LogLevel logLevel, LogLevel thresholdLevel)
        {
            return logLevel >= thresholdLevel;
        }
    }
}
