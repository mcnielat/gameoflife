using ConwaysGameOfLife.Controllers;
using ConwaysGameOfLife.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ConwaysGameOfLife.Tests
{
    public class GameControllerTests
    {

        private readonly Mock<IGameOfLife> _gameOfLifeMock;
        private readonly GameController _gameController;
        public GameControllerTests()
        {
            _gameOfLifeMock = new Mock<IGameOfLife>();
            _gameController = new GameController(_gameOfLifeMock.Object);
        }

        [Fact]
        public void Get_WithValidInput_ReturnsNextGeneration()
        {
            // Arrange
            var input = "[[0, 1, 0], [1, 1, 1], [0, 1, 0]]";
            var expectedResult = "[[1, 1, 1], [1, 0, 1], [1, 1, 1]]";

            _gameOfLifeMock.Setup(g => g.GetNextGeneration(It.IsAny<int[][]>()))
                           .Returns(expectedResult);

            // Act
            var result = _gameController.Get(input);

            // Assert
            Assert.IsType<ActionResult<string>>(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public void Get_WithEmptyInput_ReturnsBadRequest()
        {
            // Arrange
            var input = "";

            // Act
            var result = _gameController.Get(input);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public void Get_WithInvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var input = "InvalidInput";

            // Act
            var result = _gameController.Get(input);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public void Get_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            _gameOfLifeMock.Setup(g => g.GetNextGeneration(It.IsAny<int[][]>()))
                .Throws(new Exception("Simulated Exception"));
            var input = "[[0, 1, 0], [1, 1, 1], [0, 1, 0]]";

            // Act
            var result = _gameController.Get(input);

            // Assert
            Assert.IsType<StatusCodeResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, ((StatusCodeResult)result.Result).StatusCode);
        }
    }
}