using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirali.Framework
{
    public class StringHandler
    {

        /// <summary>
        /// <tooltip>
        /// Reads through a string and returns back everything before the given characters
        /// </tooltip>
        /// </summary>
        /// <param name="read">The string to read</param>
        /// <param name="stop">Any characters that should stop the stream</param>
        /// <returns></returns>
        public static string ReadUntil(string read, char[] stop)
        {
            string result = "";
            if (!String.IsNullOrEmpty(read) && stop.Length != 0)
            {
                bool contains = false;
                char first = stop[0];
                foreach (char x in read)
                {
                    foreach (char n in stop)
                    {
                        if (x == n && !contains)
                        {
                            contains = true;
                            first = n;
                        }
                    }
                }
                if (contains)
                {
                    foreach (char c in read.ToCharArray())
                    {
                        if (c != first)
                            result += c;
                        else return result;
                    }
                    return result;
                }
                else
                {
                    return read;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// <tooltip>
        /// Reads through a string and returns back everything before the given character
        /// </tooltip>
        /// </summary>
        /// <param name="read">The string to read</param>
        /// <param name="stop">That character that will stop the stream</param>
        /// <returns></returns>
        public static string ReadUntil(string read, char stop, bool leaveCharEnd = false)
        {
            string result = "";
            if (!String.IsNullOrEmpty(read) && read.Contains(stop))
            {
                foreach (char c in read.ToCharArray())
                {
                    if (c != stop)
                        result += c;
                    else
                    {
                        if (leaveCharEnd) { result += stop; }
                        return result;
                    }
                }
                return result;
            }
            else
            {
                return read;
            }
        }

        /// <summary>
        /// <tooltip>
        /// Reads values enclosed in the given characters (example) [example] {example} *example* &example&
        /// </tooltip>
        /// </summary>
        /// <param name="read"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string ReadBetween(string read, char begin, char end)
        {
            string result = "";
            string cutFirst = ReadUntil(read, begin);
            string rem = read;
            if (!string.IsNullOrEmpty(cutFirst))
                rem = rem.Remove(0, cutFirst.Length);
            rem = rem.Remove(0, 1);
            result = ReadUntil(rem, end);

            return result;
        }

        /// <summary>
        /// <tooltip>
        /// Reads through a string and returns back everything after the given character
        /// </tooltip>
        /// </summary>
        /// <param name="read">The string to read</param>
        /// <param name="stop">The character that will start the stream</param>
        /// <returns></returns>
        public static string ReadAfter(string read, char start, bool leaveCharBegin = false)
        {
            string result = "";
            if (!String.IsNullOrEmpty(read) && read.Contains(start))
            {
                if (leaveCharBegin) { result += start; }
                bool add = false;
                foreach (char c in read.ToCharArray())
                {
                    if (add)
                        result += c;
                    if (!add && c == start)
                    {
                        add = true;
                    }
                }
                return result;
            }
            else
            {
                return read;
            }
        }

        /// <summary>
        /// <tooltip>
        /// Reads through a string and returns back everything before the given string
        /// </tooltip>
        /// </summary>
        /// <param name="read"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public static string ReadUntil(string read, string stop)
        {
            string result = "";
            if (!String.IsNullOrEmpty(read) && !String.IsNullOrEmpty(stop))
            {
                foreach (char c in read.ToCharArray())
                {
                    if (result.Contains(stop))
                    {
                        result = result.Replace(stop, "");
                        return result;
                    }
                    else
                        result += c;
                }
                if (result.Contains(stop))
                {
                    result = result.Replace(stop, "");
                    return result;
                }
                return read;
            }
            else
            {
                return read;
            }
        }

        /// <summary>
        /// <tooltip>
        /// Reads values enclosed in the given strings ***example*** [[[example]]] >>>example<<<
        /// </tooltip>
        /// </summary>
        /// <param name="read"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string ReadBetween(string read, string begin, string end)
        {
            string result = "";
            string cutFirst = ReadUntil(read, begin);

            string rem = read;
            if (!string.IsNullOrEmpty(cutFirst))
                rem = read.Replace(cutFirst, "");

            result = ReadUntil(rem, end).Replace(begin.ToString(), "");

            return result;
        }


    }
}
