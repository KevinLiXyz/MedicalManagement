using FluentAssertions;
using MedicalImagingSystem.Logger;
using MedicalImagingSystem.ILogger;
using Microsoft.Extensions.Logging;
using Moq;

namespace MedicalImagingSystem.Logger.Tests
{
    public class LogManagerTests
    {
        private readonly Mock<Microsoft.Extensions.Logging.ILogger> _mockLogger;
        private readonly ILogManager _logManager;

        public LogManagerTests()
        {
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger>();
            _logManager = new LogManager(_mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithValidLogger_ShouldInitializeSuccessfully()
        {
            // Arrange & Act
            var logManager = new LogManager(_mockLogger.Object);

            // Assert
            logManager.Should().NotBeNull();
            logManager.Should().BeAssignableTo<ILogManager>();
        }

        [Fact]
        public void LogInfo_WithMessage_ShouldCallLoggerLogInformation()
        {
            // Arrange
            var message = "Test information message";

            // Act
            _logManager.LogInfo(message);

            // Assert
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void LogWarning_WithMessage_ShouldCallLoggerLogWarning()
        {
            // Arrange
            var message = "Test warning message";

            // Act
            _logManager.LogWarning(message);

            // Assert
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void LogError_WithMessageOnly_ShouldCallLoggerLogError()
        {
            // Arrange
            var message = "Test error message";

            // Act
            _logManager.LogError(message);

            // Assert
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void LogError_WithMessageAndException_ShouldCallLoggerLogErrorWithException()
        {
            // Arrange
            var message = "Test error message with exception";
            var exception = new InvalidOperationException("Test exception");

            // Act
            _logManager.LogError(message, exception);

            // Assert
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void LogInfo_WithEmptyOrWhitespaceMessage_ShouldStillLog(string message)
        {
            // Act
            _logManager.LogInfo(message);

            // Assert
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void LogWarning_WithEmptyOrWhitespaceMessage_ShouldStillLog(string message)
        {
            // Act
            _logManager.LogWarning(message);

            // Assert
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void LogError_WithEmptyOrWhitespaceMessage_ShouldStillLog(string message)
        {
            // Act
            _logManager.LogError(message);

            // Assert
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void LogInfo_WithNullMessage_ShouldHandleGracefully()
        {
            // Act
            Action action = () => _logManager.LogInfo(null!);

            // Assert
            action.Should().NotThrow();
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void LogWarning_WithNullMessage_ShouldHandleGracefully()
        {
            // Act
            Action action = () => _logManager.LogWarning(null!);

            // Assert
            action.Should().NotThrow();
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void LogError_WithNullMessage_ShouldHandleGracefully()
        {
            // Act
            Action action = () => _logManager.LogError(null!);

            // Assert
            action.Should().NotThrow();
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void MultipleLogCalls_ShouldCallLoggerMultipleTimes()
        {
            // Arrange
            var infoMessage = "Info message";
            var warningMessage = "Warning message";
            var errorMessage = "Error message";

            // Act
            _logManager.LogInfo(infoMessage);
            _logManager.LogWarning(warningMessage);
            _logManager.LogError(errorMessage);

            // Assert
            _mockLogger.Verify(
                logger => logger.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Exactly(3));
        }

        [Fact]
        public void LogError_WithNullException_ShouldNotThrow()
        {
            // Arrange
            var message = "Error message";
            Exception? nullException = null;

            // Act
            Action action = () => _logManager.LogError(message, nullException);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void LogManager_ShouldImplementILogManagerInterface()
        {
            // Assert
            _logManager.Should().BeAssignableTo<ILogManager>();
        }

        [Fact]
        public void LogManager_AllMethods_ShouldBePublic()
        {
            // Arrange
            var logManagerType = typeof(LogManager);

            // Assert
            var methods = logManagerType.GetMethods().Where(m => m.DeclaringType == logManagerType);
            methods.Should().AllSatisfy(method => method.IsPublic.Should().BeTrue());
        }
    }
}