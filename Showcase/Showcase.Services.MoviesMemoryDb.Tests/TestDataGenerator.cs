using System;
using System.Collections;
using System.Collections.Generic;

namespace Showcase.Services.MoviesMemoryDb.Tests
{
    public class TestDataGenerator : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>
    {
        new object[] {Guid.Parse("b06da2ab-56df-0000-90ee-1cff9dd181c3") },
        new object[] {Guid.Parse("b06da2ab-56df-1111-90ee-1cff9dd181c3") }
    };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
