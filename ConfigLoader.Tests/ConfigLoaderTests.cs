using GameUtils.Config;
using System.IO;

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