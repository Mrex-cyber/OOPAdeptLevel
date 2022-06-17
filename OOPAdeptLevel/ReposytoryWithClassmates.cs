
namespace OOPAdeptLevel
{
    class ReposytoryWithClassmates
    {
        string[][] classmates = new string[2][]
        {
            new string[] { "Ivan", "Stas", "Vasya", "Maks", "Valik" },
            new string[] { "Diana", "Dasha", "Vika", "Vasylyna" }
        };
              
        public async IAsyncEnumerable<string> GetDataAsync()
        {
            Console.WriteLine("\t\t\tBoys");
            for (int b = 0; b < classmates[0].Length; b++)
            {
                Console.WriteLine($"\tTake {b + 1} element");
                await Task.Delay(500);
                yield return classmates[0][b];
            }
            Console.WriteLine("\t\t\tGirls");
            for (int g = 0; g < classmates[1].Length; g++)
            {
                Console.WriteLine($"\tTake {g + 1} element");
                await Task.Delay(500);
                yield return classmates[1][g];
            }
        }
    }

}