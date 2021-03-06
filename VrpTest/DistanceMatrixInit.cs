﻿using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Google.OrTools.ConstraintSolver;
using System.Linq;
using Newtonsoft.Json;
using Google.Protobuf.WellKnownTypes;//Duration
using VrpTest.Struct;

namespace VrpTest
{  
    public partial class VrpTest
    {
        static void TimeMatrixInit(Day day, ConfigParams cfg)
        {
                create_time_matrix(day, cfg);  
        }

        static void create_time_matrix(Day day, ConfigParams cfg)
        {
            string API_key;
            int max_elements;
            int num_addresses;
            //num_addresses = q * max_rows + r , iteration
            int q, r, i;
            int max_rows;
            JsonClasses.Rootobject response;
            List<string> addresses;
            List<string> dest_addresses;
            List<string> origin_addresses;


            addresses = day.Addresses;
            API_key = cfg.API_key;


            max_elements = 100;//api limit
            num_addresses = addresses.Count;//number of addresses
            max_rows = max_elements / num_addresses;
            q = num_addresses / max_rows;//Bolum
            r = num_addresses % max_rows;//Kalan
            dest_addresses = addresses;

            for (i = 0; i < q; i++)
            {
                origin_addresses = addresses.Skip(i * max_rows).Take(max_rows).ToList();
                response = send_request(origin_addresses, dest_addresses, API_key);
                build_time_matrix(response, day, i * max_rows, max_rows);
            }

            if (r > 0)
            {
                origin_addresses = addresses.Skip(q * max_rows).Take(r).ToList();
                response = send_request(origin_addresses, dest_addresses, API_key);
                build_time_matrix(response, day, i * max_rows, r);
            }

        }


        static JsonClasses.Rootobject send_request(List<string> origin_addresses, List<string> dest_addresses, string API_key)
        {
            string request_str, origin_address_str, dest_address_str;
            JsonClasses.Rootobject response_obj;

            request_str = "https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial";
            origin_address_str = build_address_str(origin_addresses);
            dest_address_str = build_address_str(dest_addresses);

            request_str = request_str + "&origins=" + origin_address_str + "&destinations=" +
                           dest_address_str + "&key=" + API_key;


            HttpWebRequest request =
                (HttpWebRequest)WebRequest.Create(request_str);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                Console.WriteLine(result);
                //System.IO.File.WriteAllText(@"..\..\..\Docs\TimeMatrixLG.json", result);
                response_obj = JsonConvert.DeserializeObject<JsonClasses.Rootobject>(result);

            }

            /*if (!string.IsNullOrEmpty(result))
            {
            }*/
            return response_obj;


        }

        static string build_address_str(List<string> addresses)
        {
            // Build a pipe-separated string of addresses
            string address_str = "";
            int i;

            for (i = 0; i < (addresses.Count - 1); i++)
            {
                address_str += addresses[i] + '|';
            }
            address_str += addresses[addresses.Count - 1];

            return address_str;
        }


        static void build_time_matrix(JsonClasses.Rootobject response, Day day, int size, int max_rows)
        {
            int rownum = 0;
           
            for (int i = size; i < size + max_rows; i++)
            {
                
                //row_list = [row['elements'][j]['distance']['value'] for j in range(len(row['elements']))];
                for (int j = 0; j < response.rows[rownum].elements.Length; j++)
                {

                    //distance, duration
                    if (day.TimeWindowsActive != true)
                    {
                        day.TimeMatrix[i, j] = response.rows[rownum].elements[j].duration.value;
                    }
                    else
                    {
                        day.TimeMatrix[i, j] = response.rows[rownum].elements[j].duration.value/60;
                    }
                    //Console.WriteLine(data.DistanceMatrix[i,j]);
                    //day.Penalty += response.rows[rownum].elements[j].duration.value;

                }
                //distance_matrix.append(row_list);
                rownum++;

            }
        }

        //void ReduceMatrix(in DataModel data,
        //    in RoutingModel routing,
        //    in RoutingIndexManager manager,
        //    in Assignment solution,
        //    in Day day)
        //{
        //    for (int i = 0; i < data.VehicleNumber; i++)
        //    {
        //        day.Vehicles.ElementAt(i).routeDistance = 0;

        //        var index = routing.Start(i);

        //        while (routing.IsEnd(index) == false)
        //        {
        //            day.Vehicles.ElementAt(i).Route.Add(manager.IndexToNode((int)index));

        //            var previousIndex = index;
        //            index = solution.Value(routing.NextVar(index));

        //            day.Vehicles.ElementAt(i).routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
        //        }
        //        day.Vehicles.ElementAt(i).Route.Add(manager.IndexToNode((int)index));

        //    }

        //    data.locationDropped = false;

        //    List<int> droppedNodes = new List<int>();

        //    for (int index = 0; index < routing.Size(); ++index)
        //    {
        //        if (routing.IsStart(index) || routing.IsEnd(index))
        //        {
        //            continue;
        //        }
        //        if (solution.Value(routing.NextVar(index)) == index)
        //        {
        //            droppedNodes.Add(manager.IndexToNode(index));
        //            data.locationDropped = true;
        //        }
        //    }

        //    List<string> addresses = new List<string>();

        //    foreach (var item in droppedNodes)
        //    {
        //        addresses.Add(data.addresses.ElementAt(item));
        //    }

        //    long [,] TimeWindows = new long[addresses.Count, addresses.Count];

        //    foreach (var item in droppedNodes)
        //    {
        //        TimeWindows[][]
        //    }

        //}
    }
}
