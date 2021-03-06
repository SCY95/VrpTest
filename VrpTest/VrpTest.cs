using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Google.OrTools.ConstraintSolver;
using System.Linq;
using Newtonsoft.Json;
using Google.Protobuf.WellKnownTypes;//Duration
using System.Diagnostics;
using VrpTest.Struct;

namespace VrpTest
{
    public partial class VrpTest
    {       
        public static void Main(String[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

            // Instantiate the data problem.
            DataInput dataInput = new DataInput();//Config interface
            DataOutput dataOutput = new DataOutput();//Output interface
            VrpProblem vrpProblem = new VrpProblem();
            ConfigParams cfg = new ConfigParams();

            //Period(x) => period for x days     
            Period period = new Period(14);
            bool AssignToDays = true;
            int[] VCMinMax = new int[2];//Vehicle Count            
            dataInput.GetVCMinMax(VCMinMax);

            

            if (AssignToDays == true)
            {
                LocationDB.ResetVisitDays(LocationDB.Locations);                

                for (int i = 0; i < period.Days.Count; i++)
                {
                    if(i == 6 || i == 13)
                    {
                        continue;
                    }
                    GetInput(dataInput, cfg, period.Days.ElementAt(i));
                    if(i < 6)
                    {
                        period.Days.ElementAt(i).SetDay(LocationDB.Locations.Where(x => x.VisitDay == 0 && x.Infeasible == false).ToList());
                        period.Days.ElementAt(i).DayNum = i + 1;
                        AssignAndSolveForDay(dataInput, dataOutput, vrpProblem, period.Days.ElementAt(i), cfg, VCMinMax);                        
                    }
                    if(i > 6)
                    {
                        List<Location> locations = new List<Location>();
                        foreach (var item in period.Days.ElementAt(i - 7).Locations.Where(x => x.VisitPeriod == 7).ToList())
                        {
                            item.Penalty = 1000000;
                            locations.Add(item);
                        }
                        locations.AddRange(LocationDB.Locations.Where(x => x.VisitDay == 0 && x.VisitPeriod == 14 && x.Infeasible == false).ToList());

                        period.Days.ElementAt(i).SetDay(locations);
                                                
                        period.Days.ElementAt(i).DayNum = i + 1;                    

                        AssignAndSolveForDay(dataInput, dataOutput, vrpProblem, period.Days.ElementAt(i), cfg, VCMinMax);                   

                    }

                }
                period.PrintSummary();

            }
            else
            {
                SetLocationsForDays(period);

                LocationDB.GetLocationDataFromDB();
                
                for (int i = 0; i < period.Days.Count; i++)
                {                    
                    if (period.Days.ElementAt(i).Locations.Count != 0)
                    {
                        GetInput(dataInput, cfg, period.Days.ElementAt(i));
                        SolveForAssignedDay(dataInput, dataOutput, vrpProblem, period.Days.ElementAt(i), cfg, VCMinMax);
                    }

                }

                period.PrintSummary();
            }
            

           
             
            Console.ReadLine();
            return;
        }                                   
    }
}
















/*static void Main()
{


    //Pass request to google api with orgin and destination details
    HttpWebRequest request =
        (HttpWebRequest)WebRequest.Create("https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=Washington,DC&destinations=New+York+City,NY&key=AIzaSyBe5wHtu7fTIbEfls4Z-8FCkfCJcf41Udc");

    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    using (var streamReader = new StreamReader(response.GetResponseStream()))
    {
        var result = streamReader.ReadToEnd();

        if (!string.IsNullOrEmpty(result))
        {
            Console.WriteLine(result);
        }
    }

    Console.ReadLine();
    return;
}*/




//  searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
//  searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
//  searchParameters.TimeLimit = new Duration { Seconds = 7};
//  searchParameters.LogSearch = true;



//var rowCount = data.DistanceMatrix.GetLength(0);
//var colCount = data.DistanceMatrix.GetLength(1);
//for (int row = 0; row < rowCount; row++)
//{
//    for (int col = 0; col < colCount; col++)
//    {
//        Console.Write(string.Format("{0} ", data.DistanceMatrix[row, col]));

//    }
//    Console.Write(Environment.NewLine + Environment.NewLine);


//}