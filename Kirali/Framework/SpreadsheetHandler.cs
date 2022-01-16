using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirali.Framework
{
    public class SpreadsheetHandler
    {
        public static SpreadsheetHandler Empty = new SpreadsheetHandler();

        private SpreadsheetHandler()
        {
            NULL = true;
        }

        public bool NULL = true;

        private string m_CONTENTS;
        private string[] rawcontents;
        private int labeledRows = 0;
        private int labeledColumns = 0;
        private int height;
        private int width;


        public string[,] sheet;
        public int LabeledRows { get { return labeledRows; } }
        public int LabeledColumns { get { return labeledColumns; } }
        public int Height { get { return height; } }
        public int Width { get { return width; } }

        /// <summary>
        /// Creates a new multidimensional array from a tsv (tab separated values) file
        /// </summary>
        /// <param name="contents">
        /// <tooltip>
        /// the string data to input
        /// </tooltip>
        /// </param>
        /// <param name="columns"><tooltip>
        /// number of columns in the spreadsheet
        /// </tooltip></param>
        public SpreadsheetHandler(string contents, int rows, int columns, char key = '\t', int labelRow = 0, int labelColumn = 0)
        {
            labeledRows = labelRow;
            labeledColumns = labelColumn;
            string refined = contents.Replace("\r\n", key.ToString());
            m_CONTENTS = contents;
            rawcontents = refined.Split(key);
            sheet = new string[columns, rows];
            width = columns;
            height = rows;
            for (int Y = 0; Y < rows; Y++)
            {
                for (int X = 0; X < columns; X++)
                {
                    try
                    {
                        sheet[X, Y] = rawcontents[Y * columns + X];
                    }
                    catch
                    {

                    }
                }
            }

            NULL = false;

        }

        public SpreadsheetHandler(string[] contents, int rows, int columns)
        {
            height = rows;
            width = columns;
            rawcontents = contents;
            sheet = new string[columns, rows];
            for(int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    sheet[x, y] = rawcontents[x + y * columns];
                }
            }

            // :)
        }

        public SpreadsheetHandler(string[,] contents, int rows, int columns)
        {
            height = rows;
            width = columns;
            //rawcontents = contents;
            sheet = new string[columns, rows];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    sheet[x, y] = contents[x , y];
                }
            }

            // :)
        }

        public SpreadsheetHandler(int rows, int columns)
        {
            height = rows;
            width = columns;
            //rawcontents = contents;
            sheet = new string[columns, rows];
            

            // :)
        }

        public string SerializeSheet(string key = "\t")
        {
            string output = "";

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    string curcel = sheet[x, y];
                    output += curcel + key;
                }
                output += "\r\n";
            }

            return output;
        }

        public string GetCell(int row, int column)
        {
            if (!NULL)
            {
                string contents = "";
                contents = sheet[row, column];
                return contents;
            }
            else
            {
                throw new Exception("SpreadsheetHandlerNullException");
            }
        }

        public string GetCell(string row, string column)
        {
            if (!NULL)
            {
                int x = 0;
                int y = 0;
                while (x < width - 1)
                {
                    if (sheet[x, 0] == column) { break; }
                    x++;
                }
                while (y < height - 1)
                {
                    if (sheet[0, y] == row) { break; }
                    y++;
                }
                return sheet[x, y];
            }
            else
            {
                throw new Exception("SpreadsheetHandlerNullException");
            }
        }

        public string GetCell(int row, string column)
        {
            if (!NULL)
            {
                int y = 0;
                while (y < height - 1)
                {
                    if (sheet[0, y] == column) { break; }
                    y++;
                }

                return sheet[y, row];
            }
            else
            {
                throw new Exception("SpreadsheetHandlerNullException");
            }
        }

        public string GetCell(string row, int column)
        {
            if (!NULL)
            {
                int x = 0;
                while (x < width - 1)
                {
                    if (sheet[x, 0] == row) { break; }
                    x++;
                }
                return sheet[x, column];
            }
            else
            {
                throw new Exception("SpreadsheetHandlerNullException");
            }
        }
    }
}
