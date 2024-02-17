namespace DiceExercise;

class Program
{
    static void Main(string[] args)
    {
        /*optional to set iteration and dice count, default value is 10000 and 5*/
        // IDiceGame game = new DiceGameV1();
        IDiceGame game = new DiceGameV1()
                            .SetIterationCount(2500)
                            .SetDiceCount(6);
        game.Run();
    }
}
