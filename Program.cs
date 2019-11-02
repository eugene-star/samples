/*
 * Two strings, a and b, are said to be twins only if they can be made equivalent by performing some number of operations on one or both strings. There are two possible operations:

SwapEven: Swap a character at an even-numbered index with a character at another even-numbered index.
SwapOdd: Swap a character at an odd-numbered index with a character at another odd-numbered index.
For example, a = "abcd" and b = "cdab" are twins because we can make them equivalent by performing operations. Alternatively, a = "abcd" and b = "bcda" are not twins (operations do not move characters between odd and even indices), and neither are a = "abc" and b = "ab" (no amount of operations will insert a 'c' into string b).

Complete the twins function in the editor below. It has two parameters:

An array of n strings named a.
An array of n strings named b.
The function must return an array of strings where each index i (0 ≤ i < n) contains the string Yes if ai and bi are twins or the string No if they are not.

Input Format

The internal test cases read the following input from stdin and pass it to the function:

The first line contains an integer, n, denoting the number of elements in a.

Each line i of the n subsequent lines (where 0 ≤ i < n) contains a string describing ai.

The next line contains an integer, n, denoting the number of elements in b.

Each line i of the n subsequent lines (where 0 ≤ i < n) contains a string describing bi.

Constraints

1 ≤ n ≤ 10^3
1 ≤ lengths of ai, bi ≤ 100
ai and bi are not guaranteed to have the same length.
Strings ai and bi contain lowercase letters only (i.e., a through z).
Output Format

The function must return an array of strings where each index i (0 ≤ i < n) contains the string Yes if ai and bi are twins or the string No if they are not.

Sample Input :

2

cdab

dcba

2

abcd

abcd



Sample Output :

Yes

No



Explanation :

Given a = ["cdab", "dcba"] and b = ["abcd", "abcd"], we process each element like so:

a0 = "cdab" and b0 = "abcd": We store Yes in index 0 of the return array because a0 = "cdab" → "adcb" → "abcd" = b0.
a1 = "dcba" and b1 = "abcd": We store No in index 1 of the return array because no amount of operations will move a character from an odd index to an even index, so the two strings will never be equal.
We then return the array ["Yes", "No"] as our answer.

For example:

Input	Result
2
cdab
dcba
2
abcd
abcd
Yes
No

 */
using System;
using System.Collections.Generic;

namespace Trial
{
    class Program
    {
        /*
         * Complete the function below.
         * DO NOT MODIFY CODE OUTSIDE THIS FUNCTION!
         */
        static string[] twins(string[] a, string[] b)
        {
            int n = a.Length;
            if (b.Length < n)
                n = b.Length;
            var res = new string[n];

            if (n == 0)
                return res;

            for(int i=0; i<n; i++)
            {
                if(a[i] == b[i])
                    res[i] = "Yes";
                else if (a[i].Length != b[i].Length)
                    res[i] = "No";
                else
                {
                    var ai = a[i].ToCharArray();
                    var bi = b[i].ToCharArray();

                    var a_odds = new List<char>(ai.Length / 2);
                    var a_evens = new List<char>(ai.Length / 2);
                    var b_odds = new List<char>(bi.Length / 2);
                    var b_evens = new List<char>(bi.Length / 2);

                    for(int j=0; j<ai.Length; j++)
                    {
                        if(j % 2 == 0)
                        {
                            a_evens.Add(ai[j]);
                            b_evens.Add(bi[j]);
                        }
                        else
                        {
                            a_odds.Add(ai[j]);
                            b_odds.Add(bi[j]);
                        }
                    }

                    a_odds.Sort();
                    b_odds.Sort();
                    a_evens.Sort();
                    b_evens.Sort();

                    var s_a_odds = new string(a_odds.ToArray());
                    var s_b_odds = new string(b_odds.ToArray());
                    var s_a_evens = new string(a_evens.ToArray());
                    var s_b_evens = new string(b_evens.ToArray());

                    if (s_a_odds == s_b_odds && s_a_evens == s_b_evens)
                        res[i] = "Yes";
                    else
                        res[i] = "No";
                }
            }

            return res;
        }
        // DO NOT MODIFY CODE OUTSIDE THE ABOVE FUNCTION!

        static void Main(String[] args)
        {
            string[] res;

            int _a_size = 0;
            _a_size = Convert.ToInt32(Console.ReadLine());
            string[] _a = new string[_a_size];
            string _a_item;
            for (int _a_i = 0; _a_i < _a_size; _a_i++)
            {
                _a_item = Console.ReadLine();
                _a[_a_i] = _a_item;
            }


            int _b_size = 0;
            _b_size = Convert.ToInt32(Console.ReadLine());
            string[] _b = new string[_b_size];
            string _b_item;
            for (int _b_i = 0; _b_i < _b_size; _b_i++)
            {
                _b_item = Console.ReadLine();
                _b[_b_i] = _b_item;
            }

            res = twins(_a, _b);
            for (int res_i = 0; res_i < res.Length; res_i++)
            {
                Console.WriteLine(res[res_i]);
            }
        }
    }
}