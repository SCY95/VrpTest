﻿using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Google.OrTools.ConstraintSolver;
using System.Linq;
using Newtonsoft.Json;
using Google.Protobuf.WellKnownTypes;//Duration
using System.Diagnostics;

namespace VrpTest
{
    public partial class VrpTest
    {
        public static void SolveForDay(DataInput dataInput, DataOutput dataOutput, 
            VrpProblem vrpProblem, Day day,ConfigParams cfg)
        {
            
            TimeMatrixInit(day, cfg);

            vrpProblem.SolveVrpProblem(day, cfg);

            int i = 1;
            int max_vehicles = 150;
            while (day.LocationDropped && i < max_vehicles)
            {
                day.SetVehicleNumber(i);

                vrpProblem.SolveVrpProblem(day, cfg);

                dataOutput.PrintSolution(vrpProblem.day, vrpProblem.routing, vrpProblem.manager, vrpProblem.solution);

                i++;
            }

            dataOutput.PrintStatus(vrpProblem.routing);


            return;
        }
    }
}
















