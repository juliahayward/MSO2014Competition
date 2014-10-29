using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSOProgrammingApp
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var classes = textBox2.Text.Split(' ').Select(x => int.Parse(x)).ToArray();
            var upNoSwaps = countUp(classes);
            var bestUp = upNoSwaps;
            var bestMove = 0;
            for (int i = 0; i < classes.Length - 1; i++)
            {
                var t = classes[i]; classes[i] = classes[i + 1]; classes[i + 1] = t;
                var up = countUp(classes);
                if (up < bestUp)
                {
                    bestMove = i + 1;// floors are one-based
                    bestUp = up;
                }
                t = classes[i]; classes[i] = classes[i + 1]; classes[i + 1] = t;
            }

            if (bestMove == 0)
                MessageBox.Show("Best not to swap");
            else
                MessageBox.Show(bestMove.ToString()); 
        }

        private int countUp(int[] classes)
        {
            var floorsUp = 0; // initial climb does not count
            for (int i=0; i<classes.Length - 1; i++)
                if (classes[i+1] > classes[i])
                    floorsUp += classes[i+1] - classes[i];
            return floorsUp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int N = int.Parse(textBox3.Text);
            int[] divisors = new int[N+2];
            int[] lowestWithNDivisors = new int[N+2];

            for (int div = 1; div <= N; div++)
            {
                var multiple = div;
                while (multiple <= N)
                {
                    divisors[multiple] += 1;
                    multiple += div;
                }
            }

            for (int i = 1; i <= N; i++)
            {
                if (lowestWithNDivisors[divisors[i]] == 0)
                    lowestWithNDivisors[divisors[i]] = i;
            }

            var sum = 0;
            for (int i = 1; i < N; i++)
            {
                if (lowestWithNDivisors[i] != 0)
                    sum += lowestWithNDivisors[i];
            }

            MessageBox.Show("Sum of special numbers is " + sum.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var text = textBox4.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Skip(1).ToArray();
            var total = 0;
            double[] m = text.Select(x => double.Parse(x.Split(' ')[0])).ToArray(); 
            double[] b = text.Select(x => double.Parse(x.Split(' ')[1])).ToArray();
            for (int first = 0; first < m.Count() - 2; first++)
            {
                for (int second = first + 1; second < m.Count() - 1; second++)
                {
                    for (int third = second + 1; third < m.Count(); third++)
                    {
                        double intX1 = - (b[first] - b[second]) / (m[first] - m[second]);
                        double intX2 = - (b[first] - b[third]) / (m[first] - m[third]);
                        double intX3 = - (b[third] - b[second]) / (m[third] - m[second]);
                        bool intersectsYAxis = (intX1 * intX2) < 0 || (intX2 * intX3 < 0) || (intX1 * intX3 <= 0);
                        if (intersectsYAxis)
                            total++;
                    }
                }
            }
            MessageBox.Show(total.ToString() + " intersecting triangles");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var names = new[] { "", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE" };
            var word = textBox5.Text.ToUpper();
            var success = 0;
            for (int i=1; i<=9; i++)
            {
                var test = names[i];
                var wordTrial = word;
                while (test.Length > 0 && wordTrial.Length > 0)
                {
                    if (test[0] == wordTrial[0])
                    {
                        test = test.Substring(1);
                    }
                    wordTrial = wordTrial.Substring(1);
                }
                if (test == "")
                    success = i;
            }
            if (success == 0)
                MessageBox.Show("NO");
            else
                MessageBox.Show(success.ToString());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;

            var position = new SudokuPosition();
            position.PopulatePossibilities();
            position.InitialiseFromInput(textBox6.Text.Replace("-", "0"));
            position = Solve(position);

            string output = (position == null)
                    ? "No solution possible"
                    : position.ToString();
            DateTime end = DateTime.Now;
            output += Environment.NewLine + Environment.NewLine + "Took: " + end.Subtract(start).TotalSeconds + " seconds";

            MessageBox.Show(output);
        }

        /// <summary>
        /// The backtracking heart of the problem. On each iteration, make as much simple
        /// progress as we can. Then (assuming we haven't succeeded), we guess rather than
        /// trying deeper levels of logic - pick a cell and try each of its remaining 
        /// alternatives in turn. Wrong guesses will be abandoned as soon as they lead
        /// to a contradiction. If multiple solutions exist we'll find one of them (always
        /// the same one) - in principle we could continue backtracking to find them all.
        /// </summary>
        /// <param name="initialPosition"></param>
        /// <returns></returns>
        private SudokuPosition Solve(SudokuPosition initialPosition)
        {
            try
            {
                initialPosition.SolveAsFarAsBasicAllows();
            }
            catch (Exception)
            {
                // There was an inconsistency somewhere
                return null;
            }
            if (initialPosition.UnfilledCells() > 0)
            {
                var coords = initialPosition.FirstUnfilledCell();
                foreach (var possibility in coords.Item3)
                {
                    var position = initialPosition.Clone();
                    position.FillInCell(coords.Item1, coords.Item2, possibility);
                    try
                    {
                        return Solve(position);
                    }
                    catch (Exception)
                    {
                        // Possibility resulted in a contradiction somewhere down below
                        // Move on to the next
                    }
                }
                // Tried all possibilities, found nothing
                return null;
            }
            else
                return initialPosition;
        }
        

        private void button6_Click(object sender, EventArgs e)
        {
            var array1 = new[] { "ONE", "TWO", "THREE", "FOUR", "FIVE" };
            var array2 = new[] { "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE" };
            var shortest = ShortestWord("", array1);
            MessageBox.Show("Best word for 1-5 is " + shortest + " (" + shortest.Length + " letters)");
            shortest = ShortestWord("", array2);
            MessageBox.Show("Best word for 1-9 is " + shortest + " (" + shortest.Length + " letters)");
        }

        private string ShortestWord(string prefix, string[] strings)
        {
            // Done
            if (!strings.Any())
                return prefix;

            int shortestLength = int.MaxValue;
            string bestResult = "";
            for (int i = 0; i < strings.Length; i++)
            {
                var charToMoveAcross = strings[i][0];           // First char of string i
                var decapitatedStrings = strings
                    .Select(x => (x != "" && x[0] == charToMoveAcross) ? x.Substring(1) : x)
                    .Where(x => x != "").ToArray();
                // If no decapitated string contains this character then it must be optimal to take this one.
                bool thisCharIsTriviallyOptimal = !decapitatedStrings.Any(x => x.Contains(charToMoveAcross));
                // Any string starting with that character loses it...
                if (thisCharIsTriviallyOptimal)
                    return ShortestWord(prefix + charToMoveAcross, decapitatedStrings);   // append first character of string i to result
            }
            for (int i = 0; i < strings.Length; i++ )
            {
                var charToMoveAcross = strings[i][0];           // First char of string i
                var decapitatedStrings = strings
                    .Select(x => (x != "" && x[0] == charToMoveAcross) ? x.Substring(1) : x)
                    .Where(x => x != "").ToArray();
                // Any string starting with that character loses it...
                var thisResult = ShortestWord(prefix + charToMoveAcross, decapitatedStrings);   // append first character of string i to result
                if (thisResult.Length < shortestLength)
                    bestResult = thisResult;
            }
            return bestResult;
        }

    }
}
