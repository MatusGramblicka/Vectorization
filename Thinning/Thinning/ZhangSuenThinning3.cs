using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class ZhangSuenThinning3        // https://rosettacode.org/wiki/Zhang-Suen_thinning_algorithm#Java
    {
        static int[][] nbrs = new int[9][] { new int[] { 0, -1 }, new int[] { 1, -1 }, new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 0, 1 }, new int[] { -1, 1 }, new int[] { -1, 0 }, new int[] { -1, -1 }, new int[] { 0, -1 } };

        static int[][][] nbrGroups = new int[2][][] { new int[][] { new int[] { 0, 2, 4 }, new int[] { 2, 4, 6 } }, new int[][] { new int[] { 0, 2, 6 }, new int[] { 0, 4, 6 } } };

        static List<Point> toWhite = new List<Point>();

       public  static /*void*/bool[][] thinImage(bool[][] grid)      
        {
        bool firstStep = false;
        bool hasChanged;
 
        do {
            hasChanged = false;
            firstStep = !firstStep;
 
            for (int r = 1; r < grid.Length - 1; r++) {
                for (int c = 1; c < grid[0].Length - 1; c++) {
 
                   // if (grid[r][c] != '#')
                     if (grid[r][c] != true)
                        continue;

                     int nn = numNeighbors(grid, r, c);
                    if (nn < 2 || nn > 6)
                        continue;
 
                    if (numTransitions(grid, r, c) != 1)
                        continue;
 
                    if (!atLeastOneIsWhite(grid, r, c, firstStep ? 0 : 1))
                        continue;
 
                    toWhite.Add(new Point(c, r));
                    hasChanged = true;
                }
            }
 
            foreach (Point p in toWhite)
                grid[p.Y][p.X] = false/*' '*/;
            toWhite.Clear();
 
        } while (firstStep || hasChanged);

        return grid;
      //  printResult();
    }

        static int numNeighbors(bool[][] grid,int r, int c)
        {
            int count = 0;
            for (int i = 0; i < nbrs.Length - 1; i++)
                if (grid[r + nbrs[i][1]][c + nbrs[i][0]] == true/*'#'*/)
                    count++;
            return count;
        }

        static int numTransitions(bool[][] grid, int r, int c)
        {
            int count = 0;
            for (int i = 0; i < nbrs.Length - 1; i++)
                if (grid[r + nbrs[i][1]][c + nbrs[i][0]] == false/*' '*/)
                {
                    if (grid[r + nbrs[i + 1][1]][c + nbrs[i + 1][0]] == true/*'#'*/)
                        count++;
                }
            return count;
        }

        static bool atLeastOneIsWhite(bool[][] grid, int r, int c, int step)
        {
            int count = 0;
            int[][] group = nbrGroups[step];
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < group[i].Length; j++)
                {
                    int[] nbr = nbrs[group[i][j]];
                    if (grid[r + nbr[1]][c + nbr[0]] == false/*' '*/)
                    {
                        count++;
                        break;
                    }
                }
            return count > 1;
        }
        /*
        static void printResult()
        {
        for (char[] row : grid)
            System.out.println(row);
    }
        */
    

    }
}
