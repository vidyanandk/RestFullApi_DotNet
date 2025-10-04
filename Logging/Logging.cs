﻿using System;
namespace RestFullApi.Logging
{
    public class Logging: ILogging
    {
        public void Log(string message, string type)
        {
            if (type == "error")
            {
                Console.WriteLine("ERROR: "+message);
            }
            else
            {
                Console.WriteLine("INFO: " + message);
            }
            Console.WriteLine($"[{DateTime.Now}] [{type}] {message}");
        }
    }
}
