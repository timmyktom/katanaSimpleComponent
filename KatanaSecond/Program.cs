﻿using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KatanaSecond
{
    
    using AppFunc = Func<IDictionary<string, object>, Task>;

    class Program
    {
        static void Main(string[] args)
        {
            string uri = "http://localhost:8080";
            using (WebApp.Start<Startup>(uri))
            {
                Console.WriteLine("My Second Katana App Started");
                Console.ReadKey();
                Console.WriteLine("My Second Katana App Stopping");
            }
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.Use<HelloWorldComponent>();
            app.UseHelloWorld();
        }
    }

    //Extenstion Method 
    public static class AppBuilderExtenstions
    {
        public static void UseHelloWorld(this IAppBuilder app)
        {
            app.Use<HelloWorldComponent>();
        }
    }

    public class HelloWorldComponent
    {
        AppFunc _next;
        public HelloWorldComponent(Func<IDictionary<string, object>, Task> next)
        //public HelloWorldComponent(AppFunc next)
        {
            _next = next;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            var response = environment["owin.ResponseBody"] as Stream;
            using (var writer = new StreamWriter(response))
            {
                return writer.WriteAsync("Helloo I am from Hello Component!");
            }
        }
    }
}
