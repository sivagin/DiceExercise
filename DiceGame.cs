using System.Diagnostics;

namespace DiceExercise;

/* 
     Highlights:
     1) Using interface here,just in case there is any new implementation of the game, we can create another version without affecting consumer.
     2) Using Builder Pattern to change Iteration and Dice Count if required. By default, value will be 10,000 and 5
     3) Time Complexity O(m*n) where m is the no of iterations and n is the Dice Count
     4) Space Complexity is O(1) excluding the space taken for storing results.
     5) If its CPU intensive operation,we could use Parallel.For for iterations, but in this case, its not required.
 */
/*
   Sample Execution:

       -- Optional to set iteration and dice count, default value is 10000 and 5 --

       IDiceGame game = new DiceGameV1();
       game.Run();
       (or)
       IDiceGame game = new DiceGameV1()
                           .SetIterationCount(2500)
                           .SetDiceCount(6);
       game.Run();

*/
public class DiceGameV1 : IDiceGame
{
    private int _iterationCount = 10000;
    private int _diceCount = 5;

    private readonly int _DieSize = 6; // no of sides in a die
    private readonly int _DiceValuetobeIgnored = 3; // Die value which adds no score and to removed 

    private List<GameResult> gameResults = new List<GameResult>();

    public DiceGameV1 SetIterationCount(int iterationCount)
    {
        _iterationCount = iterationCount;
        return this;
    }

    public DiceGameV1 SetDiceCount(int diceCount)
    {
        _diceCount = diceCount;
        return this;
    }

    /// <summary>
    /// Main method to call for running the dice game
    /// </summary>
    public void Run()
    {
        ValidateInput();
        CalculateAndStoreResult();
        PrintResults();
    }

    /// <summary>
    /// Calculate score for each iteration, store the result and elapsed time.
    /// </summary>
    private void CalculateAndStoreResult()
    {
        int count = _iterationCount;
        while (count > 0)
        {
            var watch = Stopwatch.StartNew();
            var score = CalculateTotalScore();
            watch.Stop();
            gameResults.Add(new GameResult()
            {
                Score = score,
                TimeTaken = watch.Elapsed.TotalSeconds
            });
            count--;
        }
    }

    /// <summary>
    /// Private method to calculate the score for each iteration
    /// </summary>
    /// <returns>score</returns>
    private int CalculateTotalScore()
    {
        int currentDiceCount = _diceCount;

        var totalScore = 0;
        while (currentDiceCount > 0)
        {
            // Step 1: Roll the dice for the no of dice currently available, and store the min value and no of 3s.
            int min = Int32.MaxValue;
            int diceCounttobeIgnored = 0;
            for (int i = 0; i < currentDiceCount; i++)
            {
                var val = GetDiceNo(); // Simulating rolling dice.
                min = Math.Min(min, val);
                if (val == _DiceValuetobeIgnored)
                    diceCounttobeIgnored++;
            }

            // Step 2: if dice has 3, reduce the count by no of 3s and dont increase score,
            // else take the minimum and reduce by 1.
            if (diceCounttobeIgnored > 0)
            {
                currentDiceCount -= diceCounttobeIgnored;
            }
            else
            {
                currentDiceCount--;
                totalScore += min;
            }
        }
        return totalScore;
    }

    /// <summary>
    /// Group the game results of each Iteration by Score and find the sum of elapsed time and count.
    /// </summary>
    private void PrintResults()
    {
        Console.WriteLine(string.Format("\nNumber of Simulations was {0} using {1} dice\n", _iterationCount, _diceCount));

        var groupedResults = gameResults
                            .GroupBy(p => p.Score)
                            .Select(r => new
                            {
                                Score = r.First().Score,
                                ElapsedTime = r.Sum(c => c.TimeTaken),
                                Occurence = r.Count()
                            })
                            .OrderBy(o => o.Score);

        foreach (var result in groupedResults)
        {
            Console.WriteLine(string.Format("\tTotal {0} occurs {1} occurred {2} times",
                                result.Score, result.ElapsedTime.ToString("#.0000"), result.Occurence));
        }

        Console.WriteLine(string.Format("\nTotal Simulation took {0} seconds\n",
                        gameResults.Sum(r => r.TimeTaken).ToString("#.0000")));
    }

    private void ValidateInput()
    {
        if (_diceCount <= 0)
        {
            throw new ArgumentOutOfRangeException("Minimum No of Dice Should be 1");
        }
        if (_iterationCount <= 0)
        {
            throw new ArgumentOutOfRangeException("Minimum No of Iteration Should be 1");
        }
    }

    /// <summary>
    /// To simulate Rolling Dice, find a random number between 1 and 6.
    /// </summary>
    /// <returns></returns>
    private int GetDiceNo()
    {
        var random = new Random();
        return random.Next(1, _DieSize + 1);
    }

    private class GameResult
    {
        public int Score { get; set; }
        public double TimeTaken { get; set; }

    }
}
