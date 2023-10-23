using ConwaysGameOfLife.Services;

namespace ConwaysGameOfLife.Tests
{
    public class GameOfLifeTests
    {
        [Fact]
        public void GetNextGeneration_WithValidInput_ReturnsExpectedResult()
        {
            // Arrange
            var gameOfLife = new GameOfLife();
            var input = new int[][]
            {
                new int[] { 0, 1, 0 },
                new int[] { 1, 1, 1 },
                new int[] { 0, 1, 0 }
            };
            var expectedOutput = "[[1,1,0],[1,1,1],[1,1,0]]";

            // Act
            var result = gameOfLife.GetNextGeneration(input);

            // Assert
            Assert.Equal(expectedOutput, result);
        }

        [Fact]
        public void GetNextGeneration_WithEmptyInput_ReturnsEmptyArray()
        {
            // Arrange
            var gameOfLife = new GameOfLife();
            var input = Array.Empty<int[]>();

            // Act
            var result = gameOfLife.GetNextGeneration(input);

            // Assert
            Assert.Equal("[]", result);
        }

    }
}