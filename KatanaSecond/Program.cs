using Microsoft.Owin.Hosting;
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
            // Pipe line 1
            app.Use(async (context, next) =>
            {
                // can add a new key and pass to others
                context.Environment.Add("myPipeline", "pipleline 1");
                foreach (var pair in context.Environment)
                {
                    Console.WriteLine("{0}:{1}", pair.Key, pair.Value);
                }
                Console.WriteLine("*******************************");
                await next();
            });

            // make HelloWorldComponent invoke method async and put await code
            //app.UseHelloWorld();

            // Pipe line 2
            app.Use(async (context, next) =>
            {
                context.Environment["myPipeline"] = "pipleline 2";
                var response = context.Environment["owin.ResponseBody"] as Stream;
                using (var writer = new StreamWriter(response))
                {
                    writer.WriteLine("Helloo I am from another middle ware!");
                }
                Console.WriteLine("Pipe Line in second middleware: {0} ", context.Environment["myPipeline"]);
                Console.WriteLine("*******************************");
                Console.WriteLine("Requesting : {0}", context.Environment["owin.RequestPath"]);
                await next();
                Console.WriteLine("Response : {0}", context.Environment["owin.ResponseStatusCode"]);
                Console.WriteLine("Pipe Line in second middleware End: {0} ", context.Environment["myPipeline"]);
                Console.WriteLine("*******************************");
            });

            // Pipe line 3
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
        public HelloWorldComponent(AppFunc next)
        {
            _next = next;
        }

        public Task Invoke(IDictionary<string, object> context)
        //public async Task Invoke(IDictionary<string, object> context)
        {
            context["myPipeline"] = "pipleline 3";
            Console.WriteLine("Pipe Line in third middleware: {0}", context["myPipeline"]);
            Console.WriteLine("*******************************");
            var response = context["owin.ResponseBody"] as Stream;
            using (var writer = new StreamWriter(response))
            {
                // writer.WriteLine("Helloo I am from Hello Component!");
                return writer.WriteAsync("Helloo I am from Hello Component!");
            }
            // await _next(context);
        }
    }
}
