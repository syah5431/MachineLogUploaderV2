using FluentDateTime;
using MachineLogUploader.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MachineLogUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to SML Portal Auto Machine Log Uploader (auto MTBF start) : ");
            //Console.ReadLine();
            Console.WriteLine("Process started. . . .");

            UploadMachineLogger();
            //UpdateJushiIn();
            //UpdateJushiOut();
            Console.WriteLine("MTBF Worker Auto Start");
            try
            {
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = true;
                    myProcess.StartInfo.CreateNoWindow = false;
                    myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    myProcess.StartInfo.FileName = "D:\\smlportal\\SOKUTEI DATA PROCESSING SYSTEM\\MTBF\\SokuteiDataMigrator.exe";
                    myProcess.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Process completed");
        }

        private static void UpdateJushiOut()
        {
            using (var context = new BaInfoCenterDb())
            {
                var materialIn = (from a in context.SML_MaterialScanOut
                                  select a).ToList();
                var count = 0;
                foreach (var item in materialIn)
                {

                    var masterId = (from a in context.SML_MaterialMaster
                                    where a.Catalogue == item.Catalogue
                                    select a.CategoryID).FirstOrDefault();
                    item.CategoryId = masterId;

                    count += 2;
                }

                context.SaveChanges();
            }
        }
        private static void UpdateJushiIn()
        {
            using (var context = new BaInfoCenterDb())
            {
                var materialIn = (from a in context.SML_MaterialScanIn
                                  select a).ToList();
                var count = 0;
                foreach (var item in materialIn)
                {

                    var masterId = (from a in context.SML_MaterialMaster
                                    where a.Catalogue == item.Catalogue
                                    select a.CategoryID).FirstOrDefault();
                    item.CategoryId = masterId;

                    count += 2;
                }

                context.SaveChanges();
            }
        }

        private static void UploadMachineLogger()
        {
            using (var context = new SMLPORTALEntities())
            {
                var machineList = (from a in context.SML_Machine_Register
                                   where a.IsActive == true
                                   select a).OrderBy(a => a.Name).ToList();

                var counter = 0;
                foreach (var machine in machineList)
                {
                    var currentMonth = DateTime.Now.ToString("yyyyM");
                    Console.WriteLine($"file: {currentMonth}");
                    var machineLogs = new List<SML_Machine_Logger>();
                    var folderPath = $@"{machine.Url}\AL_{currentMonth}.txt";
                    folderPath = folderPath.Replace("\\10.30.30.16", "F:");

                    counter++;

                    //process here
                    try
                    {
                        var sr = new StreamReader(folderPath);
                        //Read the first line of text
                        var line = sr.ReadLine();

                        //Continue to read until you reach end of file -backup: !details[1].StartsWith("M")
                        while (line != null)
                        {
                            var ci = new CultureInfo("en-MY");
                            var details = line.Split(',');
                            if (details[1].Contains("Y0B62") || details[1].Contains("Y0B63") || details[2] == "Cont NG" 
                                || details[2] == "Accumulate Cont NG" || details[2].Contains("Blinking") || details[2].Contains("PR-CNT")
                                || details[2].Contains("Mingle") || details[2].Contains("Temp") || details[2].Contains("Err 2")
                                || details[2].Contains("END") || details[2].Contains("func") || details[2].Contains("wheel sh"))
                            {
                                //Read the next line
                                line = sr.ReadLine();
                                continue;
                            }
                            var changeDate = Convert.ToDateTime(details[0].Trim(), ci);
                            var log = new SML_Machine_Logger()
                            {
                                OperationDateTime = changeDate,
                                Date = changeDate.BeginningOfDay(),
                                OperationCode = details[1],
                                Operation = details[2],
                                Machine = machine.Name,
                                CreatedDate = DateTime.Now,
                                Changedate = DateTime.Now,
                                ChangedBy = "System",
                                CreatedBy = "System",
                                Remark = "AutoMachineLoggerUploader"
                            };

                            //check if data existed
                            var existed = (from a in context.SML_Machine_Logger
                                           where a.OperationDateTime == log.OperationDateTime && a.Machine == machine.Name
                                           select a).ToList();
                            if (existed.Count == 0) machineLogs.Add(log);

                            //Read the next line
                            line = sr.ReadLine();
                        }
                        //close the file
                        sr.Close();
                        //end process

                        foreach (var toSaveLog in machineLogs)
                        {
                            context.SML_Machine_Logger.Add(toSaveLog);
                            context.SaveChanges();
                        }

                        Console.WriteLine($"{counter}) {machine.Name} Upload completed: {folderPath}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{counter}) {machine.Name} Upload failed: {folderPath} Reasons: {e.Message}");
                        continue;
                    }
                }
            }
        }
    }
}
