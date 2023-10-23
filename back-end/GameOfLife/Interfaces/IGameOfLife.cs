namespace ConwaysGameOfLife.Interfaces
{
    public interface IGameOfLife
    {
        string GetNextGeneration(int[][] currentGen);
    }
}