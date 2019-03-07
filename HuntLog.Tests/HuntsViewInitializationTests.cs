using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using LightInject;
using Moq;
using HuntLog.Interfaces;
using System.IO;
using HuntLog.AppModule.Hunts;

namespace HuntLog.Tests
{
    public class HuntsViewInitializationTests : ContainerFixture
    {

        public HuntsViewModel _huntsViewModel;


        [Fact]
        public async Task ShouldSomething()
        {
            var content = ReadXmlFile("jakt");

            await _huntsViewModel.InitializeAsync();

            _huntsViewModel.HuntListItemViewModels.Count.Should().BeGreaterThan(0); 
        }


        private string ReadXmlFile(string name)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var pathtoXmlFiles = Path.Combine(baseDirectory, "..", "..", "..","..","HuntLog", "Xml", $"{name}.xml");

            pathtoXmlFiles = Path.GetFullPath(pathtoXmlFiles);



            return File.ReadAllText(pathtoXmlFiles);
        }



        internal override void Configure(IServiceRegistry serviceRegistry)
        {
            base.Configure(serviceRegistry);
            Mock<IFileUtility> fileUtilityMock = new Mock<IFileUtility>();
            serviceRegistry.RegisterInstance(fileUtilityMock.Object);
        }
    }
}
