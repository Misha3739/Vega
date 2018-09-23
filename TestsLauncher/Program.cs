using System;
using Vega.Tests.Controllers;

namespace TestsLauncher {
    class Program {
        static void Main(string[] args) {
            var tests = new VehiclesControllerTests();
            tests.CanValidateModelFields_InvalidlContact(null);
        }
    }
}
