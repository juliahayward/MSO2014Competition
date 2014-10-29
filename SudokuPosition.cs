using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSOProgrammingApp
{
    public class SudokuPosition
    {
        public int[][] digits = new int[9][];
        Dictionary<int, List<int>> possibilities = new Dictionary<int, List<int>>();

        public SudokuPosition()
        {}

        public void PopulatePossibilities()
        {
            for (int i = 0; i < 81; i++)
                possibilities.Add(i, new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        }

        public void InitialiseFromInput(string input)
        {
            var inputArr = input
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToArray();
            for (int i = 0; i < 9; i++)
            {
                digits[i] = inputArr[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.Parse(x)).ToArray();
            }

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (digits[i][j] > 0)
                        StrikeOut(i, j, digits[i][j], possibilities);
        }

        public void FillInCell(int i, int j, int value)
        {
            digits[i][j] = value;
            StrikeOut(i, j, digits[i][j], possibilities);
        }


        public int UnfilledCells()
        {
            var zeros = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (digits[i][j] == 0)
                    {
                        zeros++;
                    }
                }
            }
            return zeros;
        }

        public Tuple<int, int, List<int>> FirstUnfilledCell()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (digits[i][j] == 0)
                    {
                        return new Tuple<int,int,List<int>>(i, j, possibilities[i+9*j]);
                    }
                }
            }
            return null;
        }

        public override string ToString()
        {
            string output = "";
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    output += digits[i][j] + " ";
                }
                output += Environment.NewLine;
            }
            return output;
        }

        public SudokuPosition Clone()
        {
            var copy = new SudokuPosition();
            for (int i = 0; i < 9; i++)
            {
                copy.digits[i] = new int[9];
                for (int j = 0; j < 9; j++)
                {
                    copy.digits[i][j] = digits[i][j];
                    copy.possibilities[i + 9 * j] = new List<int>(possibilities[i + 9 * j]);
                }
            }
            return copy;
        }

        /// <summary>
        /// Repeatedly apply SolveBasic() until we stop getting anywhere
        /// </summary>
        public void SolveAsFarAsBasicAllows()
        {
            bool progressMade = false;
            do
            {
                progressMade = SolveBasic();
            }
            while (progressMade && this.UnfilledCells() > 0);
        }

        /// <summary>
        /// Try simple ways to make progress. Can be called repeatedly as progress made
        /// may open up new moves.
        /// </summary>
        /// <returns></returns>
        public bool SolveBasic()
        {
            bool progressMade = false;
            // If any cell has no possibilities left, we have no solution.
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (possibilities[i + 9 * j].Count == 0)
                    {
                        throw new Exception("no solution");
                    }
                }
            }
            // Any cells that only have 1 possibility left?
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (possibilities[i + 9 * j].Count == 1)
                    {
                        if (digits[i][j] == 0) progressMade = true;
                        digits[i][j] = possibilities[i + 9 * j].First();
                        StrikeOut(i, j, digits[i][j], possibilities);
                    }
                }
            }
            // Any rows which only have one place left for a given number?
            for (int target = 1; target < 9; target++)
            {
                for (int row = 0; row < 9; row++)
                {
                    int found = 0;
                    int foundAt = 9;
                    for (int j = 0; j < 9; j++)
                    {
                        if (possibilities[row + 9 * j].Contains(target))
                        {
                            found++;
                            foundAt = j;
                        }
                    }
                    if (found == 1)
                    {
                        if (digits[row][foundAt] == 0) progressMade = true;
                        digits[row][foundAt] = target;
                        StrikeOut(row, foundAt, target, possibilities);
                    }
                }
            }
            // Any columns which only have one place left for a given number?
            for (int target = 1; target < 9; target++)
            {
                for (int col = 0; col < 9; col++)
                {
                    int found = 0;
                    int foundAt = 9;
                    for (int i = 0; i < 9; i++)
                    {
                        if (possibilities[i + 9 * col].Contains(target))
                        {
                            found++;
                            foundAt = i;
                        }
                    }
                    if (found == 1)
                    {
                        if (digits[foundAt][col] == 0) progressMade = true;
                        digits[foundAt][col] = target;
                        StrikeOut(foundAt, col, target, possibilities);
                    }
                }
            }
            // Any 3x3 blocks which only have one place left for a given number?
            for (int target = 1; target < 9; target++)
            {
                for (int boxi = 0; boxi < 2; boxi++)
                {
                    for (int boxj = 0; boxj < 2; boxj++)
                    {
                        int found = 0;
                        int foundAtI = 9, foundAtJ = 9;
                        for (int i = 3 * boxi; i <= 3 * boxi + 2; i++)
                        {
                            for (int j = 3 * boxj; j <= 3 * boxj + 2; j++)
                            {
                                if (possibilities[i + 9 * j].Contains(target))
                                {
                                    found++;
                                    foundAtI = i;
                                    foundAtJ = j;
                                }
                            }
                        }
                        if (found == 1)
                        {
                            if (digits[foundAtI][foundAtJ] == 0) progressMade = true;
                            digits[foundAtI][foundAtJ] = target;
                            StrikeOut(foundAtI, foundAtJ, target, possibilities);
                        }
                    }
                }
            }
            return progressMade;
        }

        /// <summary>
        /// Given that we're going to place value "digit" in cell [i,j], remove that digit as a
        /// possibility for any cell in the same row, column or block, and remove all other
        /// possibilities for that cell.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="digit"></param>
        /// <param name="possibilities"></param>
        public void StrikeOut(int i, int j, int digit, Dictionary<int, List<int>> possibilities)
        {
            // Quicker than "remove all others" 
            possibilities[i + 9 * j].Clear();
            possibilities[i + 9 * j].Add(digit);

            for (int ii = 0; ii < 9; ii++)
                if (ii != i) possibilities[ii + 9 * j].Remove(digit);
            for (int jj = 0; jj < 9; jj++)
                if (jj != j) possibilities[i + 9 * jj].Remove(digit);
            for (int ii = 3 * (i / 3); ii <= 3 * (i / 3) + 2; ii++)
                for (int jj = 3 * (j / 3); jj <= 3 * (j / 3) + 2; jj++)
                    if (ii != i && jj != j) possibilities[i + 9 * jj].Remove(digit);
        }
    }
}
