using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Kirali.Storage
{
    public class Spreadsheet
    {
        private typesContainer[,] typesList;
        private object[,] contents;
        private int SizeX = 0;
        private int SizeY = 0;


        public Spreadsheet()
        {

        }

        public T GetCell<T>(int x, int y)
        {
            if(x >= 0 && x < SizeX && y >= 0 && y < SizeY)
            {
                return (T)contents[x, y];
            }
            else { throw new IndexOutOfRangeException("Indeces were not within the limits of the sheet."); }
        }

        public object GetCell(int x, int y)
        {
            if (x >= 0 && x < SizeX && y >= 0 && y < SizeY)
            {
                return contents[x, y];
            }
            else { throw new IndexOutOfRangeException("Indeces were not within the limits of the sheet."); }
        }


        public static Spreadsheet FromFile(string filepath, bool guessTypes, char newRow = '\n', char newCell = ',')
        {
            Spreadsheet spr = new Spreadsheet();
            int width = 0;
            int height = 0;
            object[,] cont;
            string co = File.ReadAllText(filepath);

            string[] lines = co.Split(newRow);
            height = lines.Length;
            if (height > 0)
            {
                string[] currLine;

                //continue, loop lines for max line width
                int horizontalCount;
                for (int line = 0; line < height; line++)
                {
                    currLine = lines[0].Split(newCell);
                    horizontalCount = currLine.Length;
                    if (horizontalCount > width && !String.IsNullOrEmpty(currLine[width]))
                    { width = horizontalCount; }
                }

                //set array, transfer values
                cont = new object[width, height];
                typesContainer[,] typ = new typesContainer[width, height];
                for (int line = 0; line < height; line++)
                {
                    currLine = lines[0].Split(newCell);
                    for (int ip = 0; ip < currLine.Length; ip++)
                    {
                        cont[line, ip] = currLine[ip];

                        if (guessTypes)
                        {
                            if(short.TryParse(currLine[ip], out _))
                            {
                                typ[line, ip] = typesContainer.SHORT;
                            }
                            else if (Int32.TryParse(currLine[ip], out _))
                            {
                                typ[line, ip] = typesContainer.INT;
                            }
                            else if(Double.TryParse(currLine[ip], out _))
                            {
                                typ[line, ip] = typesContainer.DOUBLE;
                            }
                            else if (currLine[ip] == "false" || currLine[ip] == "true")
                            {
                                typ[line, ip] = typesContainer.BOOL;
                            }
                            else if (currLine[ip].Length == 1)
                            {
                                typ[line, ip] = typesContainer.CHAR;
                            }
                            else
                            {
                                typ[line, ip] = typesContainer.STRING;
                            }
                        }
                    }
                }

                spr.contents = cont;
                spr.SizeX = width;
                spr.SizeY = height;
                spr.typesList = typ;

                return spr;
            }

            return null;
        }
        public static Spreadsheet FromFile(string filepath, char newRow = '\n', char newCell = ',')
        {
            Spreadsheet spr = new Spreadsheet();
            int width = 0;
            int height = 0;
            object[,] cont;
            string co = File.ReadAllText(filepath);

            string[] lines = co.Split(newRow);
            height = lines.Length;
            if (height > 0)
            {
                string[] currLine;
                
                //continue, loop lines for max line width
                int horizontalCount;
                for(int line = 0; line < height; line++)
                {
                    currLine = lines[0].Split(newCell);
                    horizontalCount = currLine.Length;
                    if(horizontalCount > width && !String.IsNullOrEmpty(currLine[width])) 
                    { width = horizontalCount; }
                }

                //set array, transfer values
                cont = new object[width, height];
                for (int line = 0; line < height; line++)
                {
                    currLine = lines[0].Split(newCell);
                    for(int ip = 0; ip < currLine.Length; ip++)
                    {
                        cont[line, ip] = currLine[ip];
                    }
                }

                spr.contents = cont;
                spr.SizeX = width;
                spr.SizeY = height;

                return spr;
            }

            return null;
        }

    }



    public enum typesContainer
    {
        UNKNOWN,
        STRING,
        INT,
        BOOL,
        DOUBLE,
        SHORT,
        CHAR
    }
}
