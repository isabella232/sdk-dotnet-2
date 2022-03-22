﻿using System;
using Candidly.Util;

namespace Temporal.Demos.AdHocScenarios
{
    public class Program
    {
        public static void Main(string[] _)
        {
            Console.WriteLine($"RuntimeEnvironmentInfo: \n{RuntimeEnvironmentInfo.SingletonInstance}");

            (new UseRawGrpcClient()).Run();
        }
    }
}