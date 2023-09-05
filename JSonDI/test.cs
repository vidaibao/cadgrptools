using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSonDI
{
    internal class Test
    {

        private readonly IConfiguration configuration;
        
        public Test(IConfiguration configuration) // ctor
        {
            this.configuration = configuration;
        }

        public void TestMethod() 
        { 
            var dataFromJSonFile = configuration.GetSection("Option").Value;
            Console.WriteLine(dataFromJSonFile);
            dataFromJSonFile = configuration.GetSection("Comment").Value;
            Console.WriteLine(dataFromJSonFile);

        }
    }
}
