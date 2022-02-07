using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;

using Kirali.REGS;

namespace Kirali.Celestials
{
    /// <summary>
    /// <tooltip>Galaxy storage system derived from REGS.</tooltip>
    /// </summary>
    public class RGalaxy
    {
        private string NAME;
        private bool setupIsDone = false;
        public string name { get { return NAME; } set { if (!setupIsDone) { NAME = value; } } }



        public StarData[] starpoints;
        public StarData[] customAdded;

        public RGalaxy()
        {
            starpoints = new StarData[0];
            customAdded = new StarData[0];
        }

        public static RGalaxy FromFolder(string folderpath) // add points that check and break if formatting errors occurr
        {
            RGalaxy rge = new RGalaxy();

            //get name of galaxy
            string[] folders = folderpath.Split('\\');
            string gname = folders[folders.Length - 1];
            if (String.IsNullOrEmpty(gname))
                gname = folders[folders.Length - 2];
            rge.name = gname;

            //get file list
            string[] fileLists = Directory.GetFiles(folderpath + "\\starfields", "*.starfield");
            BaseStarfield[] starFieldList = new BaseStarfield[fileLists.Length];
            int fileCount = 0;
            int totalStarCount = 0;
            foreach (string filePath in fileLists)
            {
                BaseStarfield cluster = JsonConvert.DeserializeObject<BaseStarfield>(File.ReadAllText(filePath));
                starFieldList[fileCount] = cluster;
                totalStarCount += cluster.starList.Length;
                fileCount++;
                Console.WriteLine(" Loaded starfield " + fileCount + " [" + filePath + "]");
            }

            //Add Starfield, but make sure data doesn't overlap from released systems
            int starTransferCounter = 0;
            rge.starpoints = new StarData[totalStarCount];
            for (int retrievalCt = 0; retrievalCt < fileCount; retrievalCt++)
            {
                foreach (StarData star in starFieldList[retrievalCt].starList)
                {
                    rge.starpoints[starTransferCounter] = star;
                    starTransferCounter++;
                }
                if (starFieldList[retrievalCt].MajorClassification == 11)
                {
                    Console.WriteLine(" Transferred starfield " + retrievalCt + " [MISC-CUSTOM]");
                }
                else
                {
                    Console.WriteLine(" Transferred starfield " + retrievalCt + " [Branch " + starFieldList[retrievalCt].BranchMasterId + " Classification " + starFieldList[retrievalCt].MajorClassification + "]");
                }

            }
            Console.WriteLine("Completed Retrieval of starfields.");


            Console.WriteLine("Checking for released systems...");
            fileLists = Directory.GetFiles(folderpath + "\\systems", "*.system");

            foreach (string filePath in fileLists)
            {
                StarSystemData SSD = JsonConvert.DeserializeObject<StarSystemData>(File.ReadAllText(filePath));
                rge.starpoints[SSD.fileOrderId].starName = SSD.systemName;
            }

            Console.WriteLine("Galaxy extraction complete.");
            return rge;
        }
    }
}
