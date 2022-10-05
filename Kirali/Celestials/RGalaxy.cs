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
        private static string CURRENT_VERSION = "regs_20_o";
        private string NAME;
        private string PATH = "";
        public string StoragePath { get { return PATH; } }
        private string VERSION = "regs_20_o";
        private bool setupIsDone = false;
        public string name { get { return NAME; } set { if (!setupIsDone) { NAME = value; } } }

        //All Points Storage
        public SystemPointStorage[] system_points;
        private int[] starfieldSize;

        public int[] loaded_fileOrderIDs;
        public SystemPointStorage[] loaded_systemPoints;

        //Genlevel 1, Systems have additional info in StarSystemData
        public StarSystemData[] released_systemInfo;

        public StarSystemData[] loaded_systemInfo;

        public RGalaxy()
        {
            system_points = new SystemPointStorage[0];
            //system_customAdded = new SystemPointStorage[0];
        }

        private bool DoLoadStarpoints = true;
        private bool DoLoadAllReleased = false;
        public static RGalaxy FromFolder(string folderpath, bool loadStarPoints = true, bool loadAllReleasedSystems = false) // add points that check and break if formatting errors occurr
        {
            RGalaxy rge = new RGalaxy(); //new galaxy obj
            rge.PATH = folderpath; //access location

            rge.DoLoadStarpoints = loadStarPoints;
            rge.DoLoadAllReleased = loadAllReleasedSystems;

            //get name of galaxy
            string[] folders = folderpath.Split('\\');
            string gname = folders[folders.Length - 1];
            if (String.IsNullOrEmpty(gname))
                gname = folders[folders.Length - 2];
            rge.name = gname;

            //get file list
            string[] fileLists = Directory.GetFiles(folderpath + "\\starfields", "*.starfield");
            BaseStarfield[] starFieldList = new BaseStarfield[fileLists.Length];
            rge.starfieldSize = new int[fileLists.Length];
            int fileCount = 0;
            int totalStarCount = 0;
            foreach (string filePath in fileLists)
            {
                BaseStarfield cluster = JsonConvert.DeserializeObject<BaseStarfield>(File.ReadAllText(filePath));
                starFieldList[fileCount] = cluster;
                totalStarCount += cluster.starList.Length;
                rge.starfieldSize[fileCount] = cluster.starList.Length;
                fileCount++;
                Console.WriteLine(" Loaded starfield " + fileCount + " [" + filePath + "]");
            }

            //Add Starfield, but make sure data doesn't overlap from released systems
            int starTransferCounter = 0;
            if (loadStarPoints) 
            {
                rge.system_points = new SystemPointStorage[totalStarCount];
                for (int retrievalCt = 0; retrievalCt < fileCount; retrievalCt++)
                {
                    foreach (SystemPointStorage star in starFieldList[retrievalCt].starList)
                    {
                        rge.system_points[starTransferCounter] = star;
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
            }
            

            if (loadAllReleasedSystems)
            {
                Console.WriteLine("Checking for released systems...");
                fileLists = Directory.GetFiles(folderpath + "\\systems", "*.system", SearchOption.AllDirectories);
                rge.released_systemInfo = new StarSystemData[fileLists.Length];
                int released_place = 0;

                foreach (string filePath in fileLists)
                {
                    StarSystemData SSD = JsonConvert.DeserializeObject<StarSystemData>(File.ReadAllText(filePath));
                    rge.released_systemInfo[released_place] = SSD;
                    rge.system_points[SSD.fileOrderId].starName = SSD.systemName;
                    released_place++;
                }
            }
            

            //Star Point Data exists in 


            rge.m_ConfirmSetupCondition();
            Console.WriteLine("Galaxy extraction complete.");
            return rge;
        }
        private void m_ConfirmSetupCondition() { setupIsDone = true; }


        //Search Stuff
        public int Search_SystemPoint(int fileOrderID, out SystemPointStorage point)
        {
            if(fileOrderID >= 0 && fileOrderID < system_points.Length)
            {
                point = system_points[fileOrderID];
                return (int)REGS_STD_ERROR.NO_ERROR_COMPLETE;
            }
            else
            {
                point = null;
                return (int)REGS_STD_ERROR.OUT_OF_BOUNDS_ERROR;
            }
        }
        public int Search_SystemPoint(string systemName, out SystemPointStorage point)
        {
            for(int ix = 0; ix < system_points.Length; ix++)
            {
                if(system_points[ix].starName == systemName)
                {
                    point = system_points[ix];
                    return (int)REGS_STD_ERROR.NO_ERROR_COMPLETE;
                }
            }
            point = null;
            return (int)REGS_STD_ERROR.RETURN_NO_RESULT_ERROR;
        }
        public int Search_SystemPoint(double x, double y, double z, out SystemPointStorage point)
        {
            for (int ix = 0; ix < system_points.Length; ix++)
            {
                if (system_points[ix].X == x
                    && system_points[ix].Y == y
                    && system_points[ix].Z == z)
                {
                    point = system_points[ix];
                    return (int)REGS_STD_ERROR.NO_ERROR_COMPLETE;
                }
            }
            point = null;
            return (int)REGS_STD_ERROR.RETURN_NO_RESULT_ERROR;
        }


        //Release Stuff
        public int Release_System(int fileOrderID, out StarSystemData sysData)
        {
            string systemPath = "";
            if(IsSystemReleased(fileOrderID, out systemPath))
            {
                StarSystemData SSD = JsonConvert.DeserializeObject<StarSystemData>(File.ReadAllText(systemPath));
                sysData = SSD;
                return (int)REGS_STD_ERROR.FOUND_RELEASED_SYSTEM;
            }
            else
            {
                StarSystemData SSD;
                string startingTag = "[" + SelectivePlacementInteger(fileOrderID, 9) + "]";
                if (!String.IsNullOrEmpty(system_points[fileOrderID].starName))
                {
                    systemPath = PATH + "\\systems\\" + startingTag + system_points[fileOrderID].starName + ".system";
                }
                else
                {
                    systemPath = PATH + "\\systems\\" + startingTag + ".system";
                }
                SSD = new StarSystemData();
                SSD.fileOrderId = fileOrderID;
                SSD.systemName = system_points[fileOrderID].starName;
                string content = JsonConvert.SerializeObject(SSD);
                File.WriteAllText(systemPath, content);
                sysData = SSD;
                return (int)REGS_STD_ERROR.CREATED_RELEASED_SYSTEM;
            }
        }
        public int Release_System(int fileOrderID, string customPath, out StarSystemData sysData)
        {
            string systemPath = "";
            if (IsSystemReleased(fileOrderID, out systemPath))
            {
                StarSystemData SSD = JsonConvert.DeserializeObject<StarSystemData>(File.ReadAllText(systemPath));
                sysData = SSD;
                return (int)REGS_STD_ERROR.FOUND_RELEASED_SYSTEM;
            }
            else
            {
                StarSystemData SSD;
                string startingTag = "[" + SelectivePlacementInteger(fileOrderID, 9) + "]";
                if (!String.IsNullOrEmpty(system_points[fileOrderID].starName))
                {
                    systemPath = PATH + "\\systems\\" + customPath + "\\" + startingTag + system_points[fileOrderID].starName + ".system";
                }
                else
                {
                    systemPath = PATH + "\\systems\\" + customPath + "\\" + startingTag + ".system";

                }
                systemPath = systemPath.Replace("\\\\", "\\");
                SSD = new StarSystemData();
                SSD.fileOrderId = fileOrderID;
                SSD.systemName = system_points[fileOrderID].starName;
                string content = JsonConvert.SerializeObject(SSD);
                File.WriteAllText(systemPath, content);
                sysData = SSD;
                return (int)REGS_STD_ERROR.CREATED_RELEASED_SYSTEM;
            }
        }
        public int Release_System(int fileOrderID, string systemName, string customPath, out StarSystemData sysData)
        {
            string systemPath = "";
            if (IsSystemReleased(fileOrderID, out systemPath))
            {
                StarSystemData SSD = JsonConvert.DeserializeObject<StarSystemData>(File.ReadAllText(systemPath));
                sysData = SSD;
                return (int)REGS_STD_ERROR.FOUND_RELEASED_SYSTEM;
            }
            else
            {
                StarSystemData SSD;
                string startingTag = "[" + SelectivePlacementInteger(fileOrderID, 9) + "]";
                systemPath = PATH + "\\systems\\" + customPath + "\\" + startingTag + systemName + ".system";
                system_points[fileOrderID].starName = systemName;
                systemPath = systemPath.Replace("\\\\", "\\");
                SSD = new StarSystemData();
                SSD.fileOrderId = fileOrderID;
                SSD.systemName = system_points[fileOrderID].starName;
                string content = JsonConvert.SerializeObject(SSD);
                File.WriteAllText(systemPath, content);
                sysData = SSD;
                return (int)REGS_STD_ERROR.CREATED_RELEASED_SYSTEM;
            }
        }

        public int Release_LoadedSystems(out int successCount)
        {
            if(loaded_systemPoints.Length > 0)
            {
                int savedStarcount = 0;
                for(int ix = 0; ix < loaded_systemPoints.Length; ix++)
                {
                    int saveError = Release_System(loaded_fileOrderIDs[ix], out _);
                    if ((REGS_STD_ERROR)saveError == REGS_STD_ERROR.CREATED_RELEASED_SYSTEM
                        || (REGS_STD_ERROR)saveError == REGS_STD_ERROR.FOUND_RELEASED_SYSTEM)
                    {
                        savedStarcount++;
                    }
                }
                successCount = savedStarcount;
                return (int)REGS_STD_ERROR.BULK_SAVED_CACHE;
            }
            else
            {
                successCount = 0;
                return (int)REGS_STD_ERROR.NO_LOADED_CACHE_ERROR;
            }
        }
        public bool IsSystemReleased(int fileOrderID, out string path)
        {
            //check if the system is in the system folder and released
            string startingTag = "[" + SelectivePlacementInteger(fileOrderID, 9) + "]";
            string[] fileLists = Directory.GetFiles(PATH + "\\systems", "*.system", SearchOption.AllDirectories);
            
            
            foreach (string filePath in fileLists)
            {
                if (filePath.Contains(startingTag))
                {
                    path = filePath;
                    return true;
                }
            }
            path = "";
            return false;
        }
        public bool IsSystemReleased(string SystemID, out string path)
        {
            //check if the system is in the system folder and released
            string[] fileLists = Directory.GetFiles(PATH + "\\systems", "*.system", SearchOption.AllDirectories);


            foreach (string filePath in fileLists)
            {
                string[] l = filePath.Split('\\');
                if (Kirali.Framework.StringHandler.ReadBetween(l[l.Length - 1], " ", ".system") == SystemID)
                {
                    path = filePath;
                    return true;
                }
            }
            path = "";
            return false;
        }
        public int LoadReleasedSystem(int fileOrderID, out StarSystemData SSD)
        {
            string path = "";
            if(IsSystemReleased(fileOrderID, out path))
            {
                SSD = JsonConvert.DeserializeObject<StarSystemData>(File.ReadAllText(path));
                return (int)REGS_STD_ERROR.FOUND_RELEASED_SYSTEM;
            }
            else { SSD = null; return (int)REGS_STD_ERROR.RETURN_NO_RESULT_ERROR; }
        }
        public int LoadReleasedSystem(string SystemID, out StarSystemData SSD)
        {
            string path = "";
            if (IsSystemReleased(SystemID, out path))
            {
                SSD = JsonConvert.DeserializeObject<StarSystemData>(File.ReadAllText(path));
                return (int)REGS_STD_ERROR.FOUND_RELEASED_SYSTEM;
            }
            else { SSD = null; return (int)REGS_STD_ERROR.RETURN_NO_RESULT_ERROR; }
        }

        //Save Stuff
        public static string SelectivePlacementInteger(int intvalue, int places)
        {
            string ret = "";
            for (int ct = 0; ct < places; ct++)
            {
                if (intvalue < Math.Pow(10, places - ct) && !(intvalue > Math.Pow(10, places - ct - 1)))
                {
                    ret += "0";
                }
            }
            string res = (ret + intvalue.ToString());
            return res;
        }
        public int SaveSystemData(StarSystemData SSD, bool allowAutoRelease = false)
        {
            string path = "";
            if (IsSystemReleased(SSD.fileOrderId, out path))
            {
                SSD.version = GalaxyToolbox.currentVersion;
                string content = JsonConvert.SerializeObject(SSD);
                File.WriteAllText(path, content);
                return (int)REGS_STD_ERROR.SAVED_RELEASED_SYSTEM;
            }
            else
            {
                if (allowAutoRelease)
                {
                    if ((REGS_STD_ERROR)Release_System(SSD.fileOrderId, out _) == REGS_STD_ERROR.CREATED_RELEASED_SYSTEM)
                    {
                        return SaveSystemData(SSD, false);
                    }
                    else { return (int)REGS_STD_ERROR.SYSTEM_NOT_RELEASED_ERROR; }
                }
                else { return (int)REGS_STD_ERROR.SYSTEM_NOT_RELEASED_ERROR; }
            }
        }


        public int LoadSystemPoints(SystemSearchParameter SSP)
        {
            int countFits = 0;
            int[] listFits = new int[system_points.Length];

            //search the starfields for any match.
            for(int ix = 0; ix < system_points.Length; ix++)
            {
                bool doesFit = true;
                if (SSP.SearchByChunk)
                {
                    if(system_points[ix].X < (SSP.X_chunk - SSP.ChunkSize / 2) || system_points[ix].X > (SSP.X_chunk + SSP.ChunkSize / 2)
                        || system_points[ix].Y < (SSP.Y_chunk - SSP.ChunkSize / 2) || system_points[ix].Y > (SSP.Y_chunk + SSP.ChunkSize / 2)
                        || system_points[ix].Z < (SSP.Z_chunk - SSP.ChunkSize / 2) || system_points[ix].Z > (SSP.Z_chunk + SSP.ChunkSize / 2))
                    {
                        doesFit = false;
                    }
                }
                if (SSP.SearchByProximity)
                {
                    double r = Math.Sqrt((SSP.X_prox - system_points[ix].X) * (SSP.X_prox - system_points[ix].X)
                        + (SSP.Y_prox - system_points[ix].Y) * (SSP.Y_prox - system_points[ix].Y)
                        + (SSP.Z_prox - system_points[ix].Z) * (SSP.Z_prox - system_points[ix].Z));
                    if(r > SSP.Proximity) { doesFit = false; }
                }
                if (SSP.SearchBySpectral)
                {
                    bool doesFitSpectral = false;
                    for(int sp = 0; sp < SSP.spectralClass.Length; sp++)
                    {
                        if(system_points[ix].spectralClass == SSP.spectralClass[sp])
                        { doesFitSpectral = true; }
                    }
                    if(!doesFitSpectral) { doesFit = false; }
                }
                if (doesFit)
                {
                    listFits[countFits] = ix;
                    countFits++;
                }
            }

            //reduce collection to save space, load data.
            loaded_fileOrderIDs = new int[countFits];
            loaded_systemPoints = new SystemPointStorage[countFits];
            for(int sel = 0; sel < countFits; sel++)
            {
                loaded_fileOrderIDs[sel] = listFits[sel];
                loaded_systemPoints[sel] = system_points[listFits[sel]];
            }
            return (int)REGS_STD_ERROR.NO_ERROR_COMPLETE;
        }
        public int LoadReleasedSystems(ReleasedSystemSearchParameter RSSP)
        {
            int countFits = 0;
            int[] listFits = new int[system_points.Length];
            

            //search the starfields for any STARPOINT match.
            for (int ix = 0; ix < system_points.Length; ix++)
            {
                string path = "";
                if(IsSystemReleased(ix, out path))
                {
                    bool doesFit = true;
                    bool doesAdvFit = true;

                    //regular system starpoint checks
                    if (RSSP.SearchByProximity)
                    {
                        double r = Math.Sqrt((RSSP.X_prox - system_points[ix].X) * (RSSP.X_prox - system_points[ix].X)
                            + (RSSP.Y_prox - system_points[ix].Y) * (RSSP.Y_prox - system_points[ix].Y)
                            + (RSSP.Z_prox - system_points[ix].Z) * (RSSP.Z_prox - system_points[ix].Z));
                        if (r > RSSP.Proximity) { doesFit = false; }
                    }
                    if (RSSP.SearchBySpectral)
                    {
                        bool doesFitSpectral = false;
                        for (int sp = 0; sp < RSSP.spectralClass.Length; sp++)
                        {
                            if (system_points[ix].spectralClass == RSSP.spectralClass[sp])
                            { doesFitSpectral = true; }
                        }
                        if (!doesFitSpectral) { doesFit = false; }
                    }
                    //advanced released systemData checks
                    if(doesFit) //save time by limiting checks to ones that are still possible
                        //given the first two constraints...
                    {
                        StarSystemData SSD = JsonConvert.DeserializeObject<StarSystemData>(File.ReadAllText(path));
                        if (RSSP.UseSectorID && RSSP.sectorID != SSD.sectorID) { doesAdvFit = false; }
                        if (RSSP.UseSuperfaction && RSSP.superfaction != SSD.superfaction) { doesAdvFit = false; }
                        if (RSSP.UseSystemName && RSSP.systemName != SSD.systemName) { doesAdvFit = false; }
                        if (RSSP.UseObjectsGenerated && RSSP.objectsGenerated != SSD.objectsGenerated) { doesAdvFit = false; }
                        if (RSSP.UseTotalLum && RSSP.totalLum != SSD.totalLum) { doesAdvFit = false; }
                        if (RSSP.UseDiscovered && RSSP.discovered != SSD.discovered) { doesAdvFit = false; }
                        if (RSSP.UseGeneration && RSSP.Generation != SSD.Generation) { doesAdvFit = false; }
                    }

                    //All constraints fit.
                    if (doesFit && doesAdvFit)
                    {
                        listFits[countFits] = ix;
                        countFits++;
                    }
                }
            }


            //reduce collection to save space, load data.
            loaded_systemInfo   = new StarSystemData[countFits];
            loaded_fileOrderIDs = new int[countFits];
            loaded_systemPoints = new SystemPointStorage[countFits];
            for (int sel = 0; sel < countFits; sel++)
            {
                string path;
                IsSystemReleased(listFits[sel], out path);
                StarSystemData SSD = JsonConvert.DeserializeObject<StarSystemData>(File.ReadAllText(path));
                loaded_systemInfo[sel] = SSD;
                loaded_fileOrderIDs[sel] = listFits[sel];
                loaded_systemPoints[sel] = system_points[listFits[sel]];
            }
            return (int)REGS_STD_ERROR.NO_ERROR_COMPLETE;
        }
        public int SaveLoadedReleasedSystems()
        {
            for(int sii = 0; sii < loaded_systemInfo.Length; sii++)
            {
                int err = SaveSystemData(loaded_systemInfo[sii]);//TODO add success counter?
            }
            return (int)REGS_STD_ERROR.RETURN_NO_ERROR_CHECKER;
        }
        public int LocateStarfieldContaining(int fileOrderID, out int fileNumber, out int positionInFile)
        {
            int remaining = fileOrderID;
            for(int fc = 0; fc < starfieldSize.Length; fc++)
            {
                if(remaining > starfieldSize[fc])
                { remaining -= starfieldSize[fc]; }
                else
                {
                    fileNumber = fc;
                    positionInFile = remaining;
                    return (int)REGS_STD_ERROR.FOUND_SYSTEM_POINT;
                }
            }
            fileNumber = -1;
            positionInFile = -1;
            return (int)REGS_STD_ERROR.RETURN_NO_RESULT_ERROR;
        }
        public int SaveSystemPoint(int fileOrderID)
        {
            int fileNum = 0;
            int pos = 0;
            REGS_STD_ERROR err = (REGS_STD_ERROR)LocateStarfieldContaining(fileOrderID, out fileNum, out pos);

            if(err == REGS_STD_ERROR.FOUND_SYSTEM_POINT)
            {
                //list files
                string[] fileLists = Directory.GetFiles(PATH + "\\starfields", "*.starfield");
                //load indexed
                BaseStarfield cluster = JsonConvert.DeserializeObject<BaseStarfield>(File.ReadAllText(fileLists[fileNum]));
                Console.WriteLine(" Loaded starfield for modification " + fileNum + " [" + fileLists[fileNum] + "]");
                //modify star in pos file
                cluster.starList[pos] = system_points[fileOrderID];
                //check if released
                string relPath = "";
                if(IsSystemReleased(fileOrderID, out relPath))
                {
                    StarSystemData SSD = JsonConvert.DeserializeObject<StarSystemData>(File.ReadAllText(relPath));
                    ModifySSD_UseSystemPoint(SSD, system_points[fileOrderID]);
                    File.WriteAllText(relPath, JsonConvert.SerializeObject(SSD));
                }
                Console.WriteLine(" Saved modified starfield " + fileNum + " [" + fileLists[fileNum] + "]");
                File.WriteAllText(fileLists[fileNum], JsonConvert.SerializeObject(cluster));
                return (int)REGS_STD_ERROR.SAVED_ALL_SYSTEM_DATA;
            }
            else { return (int)err; }
        }
        public void ModifySSD_UseSystemPoint(StarSystemData SSD, SystemPointStorage SPS)
        {
            //Currently only one shared attribute considering assoc using fileOrderID
            SSD.systemName = SPS.starName;
        }
        public int RenameSystem(int fileOrderID, string newName)
        {
            system_points[fileOrderID].starName = newName;
            return SaveSystemPoint(fileOrderID);
        }

        /// <summary>
        /// Returns the average primary system temperature value of the currently LOADED systems.
        /// </summary>
        /// <returns></returns>
        public double AvgSystemPrimaryTemp()
        {
            double t = 0;
            for(int lox = 0; lox < loaded_systemPoints.Length; lox++)
            {
                t += (loaded_systemPoints[lox].temp / loaded_systemPoints.Length);
            }
            return t;
        }

        //Do stuff like the generation shit

    }

    public class SystemSearchParameter //storage
    {
        private int[] search_specClass = new int[0];
        public int[] spectralClass
        {
            get { return search_specClass; }
            set { search_specClass = value; }
        }
        private double x_l = 0;
        private double y_l = 0;
        private double z_l = 0;
        private double r_l = 0;
        private double x_s = 0;
        private double y_s = 0;
        private double z_s = 0;
        private double r_s = 0;
        private bool use_prox = false;
        private bool use_chunk = false;
        public double X_prox { get { return x_l; } set { x_l = value; use_prox = true;} }
        public double Y_prox { get { return y_l; } set { y_l = value; use_prox = true;} }
        public double Z_prox { get { return z_l; } set { z_l = value; use_prox = true; } }
        public double X_chunk { get { return x_s; } set { x_s = value; use_chunk = true; } }
        public double Y_chunk { get { return y_s; } set { y_s = value; use_chunk = true; } }
        public double Z_chunk { get { return z_s; } set { z_s = value; use_chunk = true; } }
        public double Proximity { get { return r_l; } set { r_l = value; use_prox = true; } }
        public double ChunkSize { get { return r_s; } set { r_s = value; use_chunk = true; } }
        public bool SearchByProximity { get { return use_prox; } }
        public bool SearchByChunk { get { return use_chunk; } }
        public bool SearchBySpectral { get { if (search_specClass == null) { return false; } if (search_specClass.Length > 0) { return true; } else { return false; } } }
    }

    public class ReleasedSystemSearchParameter //storage
    {
        //default shit here
        private int[] search_specClass;
        public int[] spectralClass
        {
            get { return search_specClass; }
            set { search_specClass = value; }
        }
        private double x_l = 0;
        private double y_l = 0;
        private double z_l = 0;
        private double r_l = 0;
        private bool use_prox = false;
        public double X_prox { get { return x_l; } set { x_l = value; use_prox = true; } }
        public double Y_prox { get { return y_l; } set { y_l = value; use_prox = true; } }
        public double Z_prox { get { return z_l; } set { z_l = value; use_prox = true; } }
        public double Proximity { get { return r_l; } set { r_l = value; use_prox = true; } }
        public bool SearchByProximity { get { return use_prox; } }
        public bool SearchBySpectral { get { if (search_specClass.Length > 0) { return true; } else { return false; } } }


        //advanced SSD shit here
        private string m_sectorID; public string sectorID
        {
            get { return m_sectorID; }
            set { m_sectorID = value; use_sectorID = true; }
        }
        private bool use_sectorID = false;
        public bool UseSectorID { get { return use_sectorID; } }

        private string m_superfaction; public string superfaction
        {
            get { return m_superfaction; }
            set { m_superfaction = value; use_superfaction = true; }
        }
        private bool use_superfaction = false;
        public bool UseSuperfaction { get { return use_superfaction; } }

        private string m_systemName; public string systemName
        {
            get { return m_systemName; }
            set { m_systemName = value; use_systemName = true; }
        }
        private bool use_systemName = false;
        public bool UseSystemName { get { return use_systemName; } }

        private double m_totalLum; public double totalLum
        {
            get { return m_totalLum; }
            set { m_totalLum = value; use_totalLum = true; }
        }
        private bool use_totalLum = false;
        public bool UseTotalLum { get { return use_totalLum; } }

        private int m_Generation; public int Generation
        {
            get { return m_Generation; }
            set { m_Generation = value; use_Generation = true; }
        }
        private bool use_Generation = false;
        public bool UseGeneration { get { return use_Generation; } }

        private bool m_objectsGenerated; public bool objectsGenerated
        {
            get { return m_objectsGenerated; }
            set { m_objectsGenerated = value; use_objectsGenerated = true; }
        }
        private bool use_objectsGenerated = false;
        public bool UseObjectsGenerated { get { return use_objectsGenerated; } }

        private bool m_discovered; public bool discovered
        {
            get { return m_discovered; }
            set { m_discovered = value; use_discovered = true; }
        }
        private bool use_discovered = false;
        public bool UseDiscovered { get { return use_discovered; } }
    }

    public enum REGS_STD_ERROR
    {
        NO_ERROR_COMPLETE              = 0x00,
        RETURN_UNIMPLEMENTED           = 0x01,
        RETURN_NO_ERROR_CHECKER        = 0x02,
        RETURN_UNKNOWN_ERROR           = 0x03,
        CELESTIAL_OBJ_FOUND            = 0x20,
        CELESTIAL_OBJ_ADDED            = 0x21,
        CELESTIAL_ALREADY_EXIST_ERROR  = 0x22,
        FOUND_SYSTEM_POINT             = 0x3A,
        FOUND_RELEASED_SYSTEM          = 0x4A,
        CREATED_RELEASED_SYSTEM        = 0x4B,
        SAVED_RELEASED_SYSTEM          = 0x4C,
        SAVED_ALL_SYSTEM_DATA          = 0x4D,
        BULK_SAVED_CACHE               = 0x50,
        NO_LOADED_CACHE_ERROR          = 0x51,
        SYSTEM_NOT_RELEASED_ERROR      = 0x52,
        RETURN_NO_RESULT_ERROR         = 0x53,
        OUT_OF_BOUNDS_ERROR            = 0x5B,
    }
}
