using ConfigLoader.Tests.Models;
using GameUtils.Config;

namespace ConfigLoader.Tests
{
    public class ConfigLoaderTests
    {
        [Fact]
        public void GetShouldReturnSerializedJsonWithDefaultPath()
        {
            // setup
            var configProvider = new ConfigProvider();

            // run
            var result = configProvider.Get<TestItems>();

            // assert
            Assert.Equal("Foo", result.Name);
        }

        [Fact]
        public void GetShouldReturnSerializedJsonWithCustomPath()
        {
            // setup
            var configProvider = new ConfigProvider("TestItems");

            // run
            var result = configProvider.Get<TestItems>();

            // assert
            Assert.Equal("Foo", result.Name);
        }

        [Fact]
        public void GetShouldReturnSerializedJsonsWithCustomPaths()
        {
            // setup
            var configProvider = new ConfigProvider(new List<string> { "TestItems", "OtherTestItems" });

            // run
            var result1 = configProvider.Get<TestItems>();
            var result2 = configProvider.Get<OtherTestItems>();

            // assert
            Assert.Equal("Foo", result1.Name);
            Assert.Equal("Other", result2.Name);
        }

        [Fact]
        public void GetShouldReturnSerializedJsonWithDelegateConstructor()
        {
            // setup
            var files = Directory.GetFiles(".", "*.json");
            var configProvider = new ConfigProvider(() => 
            {
                return files.ToDictionary(f => Path.GetFileNameWithoutExtension(f),
                    f => ConfigProvider.LoadContent(f));
            });

            // run
            var result = configProvider.Get<TestItems>();

            // assert
            Assert.Equal("Foo", result.Name);
        }
    }
}