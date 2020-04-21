using System;

namespace Showcase.Services.IntegrationTests.Models
{
    public class TestGuidDeserialization
    {
        public TestGuidDeserialization()
        {


        }
        public TestGuidDeserialization(string guid)
        {
            Guid = Guid.Parse(guid);
        }
        public Guid Guid { get; set; }
    }
}
