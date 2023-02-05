using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirali.Framework
{
    public class ArrayHandler
    {
        /// <summary>
        /// Returns a new array where every place has been set to the same value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        public static T[] SetAll<T>(int size, T set)
        {
            T[] result = new T[size];
            for(int place = 0; place < size; place++)
            {
                result[place] = set;
            }
            return result;
        }

        /// <summary>
        /// <tooltip>
        /// Returns the array with the second value appended to the end.
        /// Arrays can be started with length of 0 once initialised. (use T[] array = new T[0])
        /// </tooltip>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public static T[] append<T>(T[] input, T add)
        {
            T[] output;
            if (input != null && input.Length != 0)
            {
                output = new T[input.Length + 1];
                for (int c = 0; c < input.Length; c++)
                {
                    output[c] = input[c];
                }
                output[input.Length] = add;
            }
            else
            {
                output = new T[1];
                output[0] = add;
            }

            return output;
        }


        /// <summary>
        /// <tooltip>
        /// Returns the array with indexes moved to the left, the final value replaced with the second parameter.
        /// </tooltip>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="displacer"></param>
        /// <returns></returns>
        public static T[] displace<T>(T[] array, T displacer)
        {
            T[] newArray = new T[array.Length];
            for (int c = 1; c <= array.Length; c++)
            {
                if (c != array.Length)
                {
                    newArray[c - 1] = array[c];
                }
                else
                {
                    newArray[c - 1] = displacer;
                }
            }

            return newArray;
        }

        /// <summary>
        /// <tooltip>
        /// Returns the multidimensional array with the selected row replaced.
        /// </tooltip>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="row"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T[,] setRow<T>(T[,] matrix, T[] row, int index)
        {
            int x = 0;
            while (x < row.Length)
            {
                try
                {
                    matrix[x, index] = row[x];

                }
                catch { }
                x++;
            }

            return matrix;
        }

        /// <summary>
        /// <tooltip>
        /// Returns the multidimensional array with the selected column replaced.
        /// </tooltip>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="column"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T[,] setColumn<T>(T[,] matrix, T[] column, int index)
        {
            int y = 0;
            while (y < column.Length)
            {
                try
                {
                    matrix[index, y] = column[y];

                }
                catch { }
                y++;
            }

            return matrix;
        }

        /// <summary>
        /// <tooltip>
        /// Returns the searched row.
        /// </tooltip>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="Length"></param>
        /// <param name="indexY"></param>
        /// <returns></returns>
        public static T[] getRow<T>(T[,] input, int Length, int indexY)
        {
            T[] output = new T[Length];
            for (int x = 0; x < Length; x++)
            {
                output[x] = input[x, indexY];
            }

            return output;
        }

        /// <summary>
        /// <tooltip>
        /// Returns the searched column.
        /// </tooltip>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="Length"></param>
        /// <param name="indexX"></param>
        /// <returns></returns>
        public static T[] getColumn<T>(T[,] input, int Length, int indexX)
        {
            T[] output = new T[Length];
            for (int y = 0; y < Length; y++)
            {
                output[y] = input[indexX, y];
            }

            return output;
        }

        /// <summary>
        /// <tooltip>
        /// Returns the combination of input1 + input2, where the two sets are connected end-to-end.
        /// </tooltip>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input1"></param>
        /// <param name="input2"></param>
        /// <returns></returns>
        public static T[] add<T>(T[] input1, T[] input2)
        {
            T[] output;
            if (input1 != null && input1.Length != 0 && input2 != null && input2.Length != 0)
            {
                output = new T[input1.Length + input2.Length];
                for (int c = 0; c < input1.Length; c++)
                {
                    output[c] = input1[c];
                }
                for (int c = 0; c < input2.Length; c++)
                {
                    output[c + input1.Length] = input2[c];
                }
            }
            else
            {
                output = new T[0];
            }

            return output;
        }

        /// <summary>
        /// <tooltip>
        /// Returns the array with the searched object removed.
        /// </tooltip>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="objRemove"></param>
        /// <returns></returns>
        public static T[] deleteAll<T>(T[] input, T objRemove)
        {
            T[] finalArray = new T[0];
            foreach (T val in input)
            {
                if (val.ToString() != objRemove.ToString())
                {
                    finalArray = append(finalArray, val);
                }
            }

            return finalArray;
        }

        /// <summary>
        /// <tooltip>
        /// Returns the array with the searched object removed.
        /// </tooltip>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="objRemove"></param>
        /// <returns></returns>
        public static T[] delete<T>(T[] input, T objRemove)
        {
            T[] finalArray = new T[0];
            bool found = false;
            foreach (T val in input)
            {
                if (val.ToString() != objRemove.ToString())
                {
                    finalArray = append(finalArray, val);
                }
                else if (found)
                {
                    finalArray = append(finalArray, val);
                }
            }

            return finalArray;
        }

        /// <summary>
        /// <tooltip>
        /// Returns the array with input2 appended to the end, only if input2 does not exist in the input.
        /// </tooltip>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="input2"></param>
        /// <returns></returns>
        public static T[] appendIfNonexist<T>(T[] input, T input2)
        {
            bool exist = false;
            T[] finalArray = new T[0];
            foreach (T item in input)
            {
                if (item.ToString() == input2.ToString())
                {
                    exist = true;
                    return input;
                }
            }
            if (!exist)
            {
                finalArray = append(input, input2);
                return finalArray;
            }
            else
            {
                return input;
            }
        }

        public static string sumNext(char[] input, int start, int length)
        {
            string output = "";
            for (int c = start; c < (start + length); c++)
            {
                output += input[c];
            }
            return output;
        }

        /// <summary>
        /// <tooltip>
        /// Returns an array of length [length] filled with duplicate values of [value]
        /// </tooltip>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] setAll<T>(T value, int length)
        {
            T[] result = new T[length];
            for (int c = 0; c < length; c++)
            {
                result[c] = value;
            }

            return result;
        }

    }
}
