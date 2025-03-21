﻿using System;
using System.Text;
using TNoodle.Utils;
using ArrayExtension = TNoodle.Utils.ArrayExtension;

namespace TNoodle.Solvers
{
    public class TwoByTwoSolver
    {
        /***
        There are 8 "corner" cubies, numbered 0 to 7.
        The home positions of these cubies are labeled in the diagram below.
        Each corner cubie has three exposed faces, so there are three faces labelled with each number.
        Asterisks mark the primary facelet position.
        Orientation will be the number of clockwise rotations the primary facelet is from the primary facelet position where it is located.
        We take as a convention that the BLD corner (7) is always solved, because as there are no centers, we can orient the puzzle the way we want.
        Because of that, we only need to store information about the seven first corners (from 0 to 6).
        Also, we can allow only U, R and F moves without loosing any solution.

                   +----------+
                   |*3*    *2*|
                   |    U     |
                   |*1*    *0*|
        +----------+----------+----------+----------+
        | 3      1 | 1      0 | 0      2 | 2      3 |
        |     L    |    F     |    R     |    B     |
        | 7      5 | 5      4 | 4      6 | 6      7 |
        +----------+----------+----------+----------+
                   |*5*    *4*|
                   |    D     |
                   |*7*    *6*|
                   +----------+

         ***/

        private const int N_PERM = 5040; // Number of permutations
        private const int N_ORIENT = 729; // Number of orientations
        private const int N_MOVES = 9; // Number of moves
        private const int MAX_LENGTH = 20; // Max length of solutions
        private static readonly string[] moveToString = { "U", "U2", "U'", "R", "R2", "R'", "F", "F2", "F'" };
        private static readonly string[] inverseMoveToString = { "U'", "U2", "U", "R'", "R2", "R", "F'", "F2", "F" };
        private static readonly int[] fact = { 1, 1, 2, 6, 24, 120, 720 }; // fact[x] = x!

        /**
         * Converts the list of cubies into a number representing the permutation of the cubies.
         * @param cubies   cubies representation (ori << 3 + perm)
         * @return         an integer between 0 and 5039 representing the permutation of 7 elements
         */
        public static int PackPerm(int[] cubies)
        {
            int idx = 0;
            int val = 0x6543210;
            for (int i = 0; i < 6; i++)
            {
                int v = (cubies[i] & 0x7) << 2;
                idx = (7 - i) * idx + ((val >> v) & 0x7);
                val -= 0x1111110 << v;
            }
            return idx;
        }

        /**
         * Converts an integer representing a permutation of 7 elements into a list of cubies.
         * @param perm     an integer between 0 and 5039 representing the permutation of 7 elements
         * @param cubies   cubies representation (ori << 3 + perm)
         */
        public static void UnpackPerm(int perm, int[] cubies)
        {
            int val = 0x6543210;
            for (int i = 0; i < 6; i++)
            {
                int p = fact[6 - i];
                int v = perm / p;
                perm -= v * p;
                v <<= 2;
                cubies[i] = (val >> v) & 0x7;
                int m = (1 << v) - 1;
                val = (val & m) + ((val >> 4) & ~m);
            }
            cubies[6] = val;
        }

        /**
         * Converts the list of cubies into a number representing the orientation of the cubies.
         * @param cubies   cubies representation (ori << 3 + perm)
         * @return         an integer between 0 and 728 representing the orientation of 6 elements (the 7th is fixed)
         */
        public static int PackOrient(int[] cubies)
        {
            int ori = 0;
            for (int i = 0; i < 6; i++)
            {
                ori = 3 * ori + (cubies[i] >> 3);
            }
            return ori;
        }

        /**
         * Converts an integer representing the orientation of 6 elements into a list of cubies.
         * @param ori      an integer between 0 and 728 representing the orientation of 6 elements (the 7th is fixed)
         * @param cubies   cubies representation (ori << 3 + perm)
         */
        public static void UnpackOrient(int ori, int[] cubies)
        {
            int sum_ori = 0;
            for (int i = 5; i >= 0; i--)
            {
                cubies[i] = (ori % 3) << 3;
                sum_ori += ori % 3;
                ori /= 3;
            }
            cubies[6] = ((42424242 - sum_ori) % 3) << 3; // Any number multiple of 3 greater than 15 works :)
        }

        /**
         * Cycle four elements of an array.
         * @param cubies   cubies representation (ori << 3 + perm)
         * @param a        first element to cycle
         * @param b        second element to cycle
         * @param c        third element to cycle
         * @param d        fourth element to cycle
         * @param times    number of times to cycle
         */
        private static void Cycle(int[] cubies, int a, int b, int c, int d, int times)
        {
            int temp = cubies[d];
            cubies[d] = cubies[c];
            cubies[c] = cubies[b];
            cubies[b] = cubies[a];
            cubies[a] = temp;
            if (times > 1)
            {
                Cycle(cubies, a, b, c, d, times - 1);
            }
        }

        /**
         * Cycle four elements of an array. Also orient clockwise first and third elements and counter-clockwise the other ones.
         * @param cubies   cubies representation (ori << 3 + perm)
         * @param a        first element to cycle
         * @param b        second element to cycle
         * @param c        third element to cycle
         * @param d        fourth element to cycle
         * @param times    number of times to cycle
         */
        private static void CycleAndOrient(int[] cubies, int a, int b, int c, int d, int times)
        {
            int temp = cubies[d];
            cubies[d] = (cubies[c] + 8) % 24;
            cubies[c] = (cubies[b] + 16) % 24;
            cubies[b] = (cubies[a] + 8) % 24;
            cubies[a] = (temp + 16) % 24;
            if (times > 1)
            {
                CycleAndOrient(cubies, a, b, c, d, times - 1);
            }
        }

        /**
         * Apply a move on the cubies representation.
         * @param cubies   cubies representation (ori << 3 + perm)
         * @param move     move to apply to the cubies
         */
        private static void MoveCubies(int[] cubies, int move)
        {
            int face = move / 3;
            int times = (move % 3) + 1;
            switch (face)
            {
                case 0: // U face
                    Cycle(cubies, 1, 3, 2, 0, times);
                    break;
                case 1: // R face
                    CycleAndOrient(cubies, 0, 2, 6, 4, times);
                    break;
                case 2: // F face
                    CycleAndOrient(cubies, 1, 0, 4, 5, times);
                    break;
                default:
                    //azzert(false);
                    break;
            }
        }

        /**
         * Fill the arrays to move permutation and orientation coordinates.
         */
		private static readonly int[][] movePerm = ArrayExtension.New<int>(N_PERM, N_MOVES);
		private static readonly int[][] moveOrient = ArrayExtension.New<int>(N_ORIENT, N_MOVES);
        private static void InitMoves()
        {
            int[] cubies1 = new int[7];
            int[] cubies2 = new int[7];
            for (int perm = 0; perm < N_PERM; perm++)
            {
                UnpackPerm(perm, cubies1);
                for (int move = 0; move < N_MOVES; move++)
                {
                    Array.Copy(cubies1, 0, cubies2, 0, 7);
                    MoveCubies(cubies2, move);
                    int newPerm = PackPerm(cubies2);
                    movePerm[perm][move] = newPerm;
                }
            }

            for (int orient = 0; orient < N_ORIENT; orient++)
            {
                UnpackOrient(orient, cubies1);
                for (int move = 0; move < N_MOVES; move++)
                {
                    Array.Copy(cubies1, 0, cubies2, 0, 7);
                    MoveCubies(cubies2, move);
                    int newOrient = PackOrient(cubies2);
                    moveOrient[orient][move] = newOrient;
                }
            }
        }

        /**
         * Fill the pruning tables for the permutation and orientation coordinates.
         */
        private static readonly int[] prunPerm = new int[N_PERM];
        private static readonly int[] prunOrient = new int[N_ORIENT];
        private static void InitPrun()
        {
            for (int perm = 0; perm < N_PERM; perm++)
            {
                prunPerm[perm] = -1;
            }
            prunPerm[0] = 0;

            int done = 1;
            for (int length = 0; done < N_PERM; length++)
            {
                for (int perm = 0; perm < N_PERM; perm++)
                {
                    if (prunPerm[perm] == length)
                    {
                        for (int move = 0; move < N_MOVES; move++)
                        {
                            int newPerm = movePerm[perm][move];
                            if (prunPerm[newPerm] == -1)
                            {
                                prunPerm[newPerm] = length + 1;
                                done++;
                            }
                        }
                    }
                }
            }

            for (int orient = 0; orient < N_ORIENT; orient++)
            {
                prunOrient[orient] = -1;
            }
            prunOrient[0] = 0;

            done = 1;
            for (int length = 0; done < N_ORIENT; length++)
            {
                for (int orient = 0; orient < N_ORIENT; orient++)
                {
                    if (prunOrient[orient] == length)
                    {
                        for (int move = 0; move < N_MOVES; move++)
                        {
                            int newOrient = moveOrient[orient][move];
                            if (prunOrient[newOrient] == -1)
                            {
                                prunOrient[newOrient] = length + 1;
                                done++;
                            }
                        }
                    }
                }
            }
        }

        static TwoByTwoSolver()
        {
            InitMoves();
            InitPrun();
        }

        /**
         * Search a solution from a position given by permutation and orientation coordinates
         * @param perm       permutation coordinate to solve
         * @param orient     orientation coordinate to solve
         * @param depth      current depth of the search (first called with 0)
         * @param length     the remaining number of moves we can apply
         * @param last_move  what was the last move done (first called with an int >= 9)
         * @param solution   the array containing the current moves done.
         */
        private bool Search(int perm, int orient, int depth, int length, int last_move, int[] solution, int[] best_solution)
        {
            /* If there are no moves left to try (length=0), check if the current position is solved */
            if (length == 0)
            {
                if ((perm == 0) && (orient == 0))
                {
                    // Solution found! Compute the cost of applying the reverse solution.
                    int cost = ComputeCost(solution, depth, 0, 0);
                    // We found a better solution, storing it.
                    if (cost < best_solution[depth])
                    {
                        Array.Copy(solution, 0, best_solution, 0, depth);
                        best_solution[depth] = cost;
                    }
                    return true;
                }
                return false;
            }

            /* Check if we might be able to solve the permutation or the orientation of the position
             * given the remaining number of moves ('length' parameter), using the pruning tables.
             * If not, there is no point keeping searching for a solution, just stop.
             */
            if ((prunPerm[perm] > length) || (prunOrient[orient] > length))
            {
                return false;
            }

            /* The recursive part of the search function.
             * Try every move from the current position, and call the search function with the new position
             * and the updated parameters (depth -> depth+1; length -> length-1; last_move -> move)
             * We don't need to try a move of the same face as the last move.
             */
            bool solutionFound = false;
            for (int move = 0; move < N_MOVES; move++)
            {
                // Check if the tested move is of the same face as the previous move (last_move).
                if ((move / 3) == (last_move / 3))
                {
                    continue;
                }
                // Apply the move
                int newPerm = movePerm[perm][move];
                int newOrient = moveOrient[orient][move];
                // Store the move
                solution[depth] = move;
                // Call the recursive function
                solutionFound |= Search(newPerm, newOrient, depth + 1, length - 1, move, solution, best_solution);
            }
            return solutionFound;
        }

        /**
         * Generate a random 2x2 position.
         * @param r         random int generator
         */
        public TwoByTwoState RandomState(Random r)
        {
            TwoByTwoState state = new TwoByTwoState();
            state.Permutation = r.Next(N_PERM);
            state.Orientation = r.Next(N_ORIENT);
            return state;
        }

        /**
         * Solve a given position in less than or equal to length number of turns.
         * Returns either the solution or the generator (inverse solution)
         * @param state     state
         * @param length    length of the desired solution
         * @return          a string representing the solution or the scramble of a random position
         */
        public string SolveIn(TwoByTwoState state, int length)
        {
            return Solve(state, length, false, false);
        }

        /**
         * Return a generator of a given position in exactly length number of turns or not at all.
         * Returns either the solution or the generator (inverse solution)
         * @param perm      permutation
         * @param orient    random int generator
         * @param length    length of the desired solution
         * @return          a string representing the solution or the scramble of a random position
         */
        public string GenerateExactly(TwoByTwoState state, int length)
        {
            return Solve(state, length, true, true);
        }

        private string Solve(TwoByTwoState state, int desiredLength, bool exactLength, bool inverse)
        {
            int[] solution = new int[MAX_LENGTH];
            int[] best_solution = new int[MAX_LENGTH + 1];
            bool foundSolution = false;
            int length = exactLength ? desiredLength : 0;
            while (length <= desiredLength)
            {
                best_solution[length] = 42424242;
                if (Search(state.Permutation, state.Orientation, 0, length, 42, solution, best_solution))
                {
                    foundSolution = true;
                    break;
                }
                length++;
            }

            if (!foundSolution)
            {
                return null;
            }

            if (length == 0)
            {
                return "";
            }

            StringBuilder scramble = new StringBuilder(MAX_LENGTH * 3);
            if (inverse)
            {
                scramble.Append(inverseMoveToString[best_solution[length - 1]]);
                for (int l = length - 2; l >= 0; l--)
                {
                    scramble.Append(" ").Append(inverseMoveToString[best_solution[l]]);
                }
            }
            else
            {
                scramble.Append(moveToString[best_solution[0]]);
                for (int l = 1; l < length; l++)
                {
                    scramble.Append(" ").Append(moveToString[best_solution[l]]);
                }
            }

            return scramble.ToString();
        }

        private const int cost_U = 8;
        private const int cost_U_low = 20; // when grip = -1
        private const int cost_U2 = 10;
        private const int cost_U3 = 7;
        private const int cost_R = 6;
        private const int cost_R2 = 10;
        private const int cost_R3 = 6;
        private const int cost_F = 10;
        private const int cost_F2 = 30;
        private const int cost_F3 = 19;
        private const int cost_regrip = 20;

        /**
         * Try to evaluate the cost of applying a scramble
         * @param solution      the solution found by the solver, we need to read it backward and inverting the moves
         * @param index         current position of reading. Starts at the length-1 of the solution, and decrease by 1 every call
         * @param current_cost  current cost of the sequence that has been already read
         * @param grip          state of the grip of the right hand. -1: thumb on D, 0: thumb on F, 1: thumb on U
         * @return              returns the cost of the whole sequence
         */
        private int ComputeCost(int[] solution, int index, int current_cost, int grip)
        {
            // If we are finished, just output the cost.
            if (index < 0)
            {
                return current_cost;
            }
            switch (solution[index])
            {
                case 0: // U'
                    return ComputeCost(solution, index - 1, current_cost + cost_U3, grip);
                case 1: // U2
                    return ComputeCost(solution, index - 1, current_cost + cost_U2, grip);
                case 2: // U
                    if (grip == 0)
                    {
                        return ComputeCost(solution, index - 1, current_cost + cost_U, 0);
                    }
                    else if (grip == -1)
                    {
                        return Math.Min(ComputeCost(solution, index - 1, current_cost + cost_regrip + cost_U, 0),
                                         ComputeCost(solution, index - 1, current_cost + cost_U_low, grip));
                    }
                    else
                    {
                        return ComputeCost(solution, index - 1, current_cost + cost_regrip + cost_U, 0);
                    }
                case 3: // R'
                    if (grip > -1)
                    {
                        return ComputeCost(solution, index - 1, current_cost + cost_R3, grip - 1);
                    }
                    else
                    {
                        return ComputeCost(solution, index - 1, current_cost + cost_regrip + cost_R3, -1);
                    }
                case 4: // R2
                    if (grip != 0)
                    {
                        return ComputeCost(solution, index - 1, current_cost + cost_R2, -grip);
                    }
                    else
                    {
                        return Math.Min(ComputeCost(solution, index - 1, current_cost + cost_regrip + cost_R2, -1),
                                         ComputeCost(solution, index - 1, current_cost + cost_regrip + cost_R2, 1));
                    }
                case 5: // R
                    if (grip < 1)
                    {
                        return ComputeCost(solution, index - 1, current_cost + cost_R, grip + 1);
                    }
                    else
                    {
                        return ComputeCost(solution, index - 1, current_cost + cost_regrip + cost_R, 1);
                    }
                case 6: // F'
                    if (grip != 0)
                    {
                        return ComputeCost(solution, index - 1, current_cost + cost_F3, grip);
                    }
                    else
                    {
                        return Math.Min(ComputeCost(solution, index - 1, current_cost + cost_regrip + cost_F3, -1),
                                         ComputeCost(solution, index - 1, current_cost + cost_regrip + cost_F3, 1));
                    }
                case 7: // F2
                    if (grip == -1)
                    {
                        return ComputeCost(solution, index - 1, current_cost + cost_F2, -1);
                    }
                    else
                    {
                        return ComputeCost(solution, index - 1, current_cost + cost_regrip + cost_F2, -1);
                    }
                case 8: // F
                    if (grip == -1)
                    {
                        return ComputeCost(solution, index - 1, current_cost + cost_F, -1);
                    }
                    else
                    {
                        return ComputeCost(solution, index - 1, current_cost + cost_regrip + cost_F, -1);
                    }
            }
            return -1;
        }
    }

    public class TwoByTwoState
    {
        public int Permutation { get; set; }
        public int Orientation { get; set; }
    }
}
