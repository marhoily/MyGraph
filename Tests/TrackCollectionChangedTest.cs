using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public sealed class TrackCollectionChangedTest
    {
        sealed class Sample : PropertyChangedBase
        {
            private ObservableCollection<int> _collection = new ObservableCollection<int> { 1, 2, 3 };

            public ObservableCollection<int> Collection
            {
                get { return _collection; }
                set
                {
                    if (Equals(value, _collection)) return;
                    _collection = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        private readonly Sample _sample = new Sample();
        private readonly List<string> _log = new List<string>();
        private readonly Action _dispose;

        private Action TrackSample(Action<int> added, Action<int> removed) =>
            MyGraph.ExpressionExtensions.Track(() => _sample.Collection, added, removed);

        public TrackCollectionChangedTest()
        {
            _dispose = TrackSample(
                x => _log.Add("Added: " + x),
                y => _log.Add("Removed: " + y));
            _log.Should().Equal("Added: 1", "Added: 2", "Added: 3");
            _log.Clear();
        }

        [Fact]
        public void Add_Should_Log()
        {
            _sample.Collection.Add(4);
            _log.Should().Equal("Added: 4");
        }
        [Fact]
        public void Remove_Should_Log()
        {
            _sample.Collection.RemoveAt(1);
            _log.Should().Equal("Removed: 2");
        }
        [Fact]
        public void Others_Should_Throw()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _sample.Collection.Clear());
        }
        [Fact]
        public void Dispose_Should_Stop_Logging()
        {
            _dispose();
            _sample.Collection.RemoveAt(1);
            _log.Should().BeEmpty();
        }
        [Fact]
        public void Property_Change_Should_Log()
        {
            _sample.Collection = new ObservableCollection<int> { 7 };
            _log.Should().Equal("Removed: 1", "Removed: 2", "Removed: 3", "Addded: 7");
        }
    }
}