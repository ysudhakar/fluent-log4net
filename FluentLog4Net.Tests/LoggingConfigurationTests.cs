using System;

using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;

using NUnit.Framework;

namespace FluentLog4Net
{
    [TestFixture]
    public class LoggingConfigurationTests
    {
        [SetUp]
        public void ResetConfiguration()
        {
            LogManager.GetRepository().ResetConfiguration();
        }

        [Test]
        public void RootLoggingLevel()
        {
            Log4Net.Configure()
                .Logging.Root(log => log.At(Level.Severe))
                .ApplyConfiguration();

            var repo = (Hierarchy)LogManager.GetRepository();
            Assert.That(repo.Root.Level, Is.EqualTo(Level.Severe));
        }

        [Test]
        public void NullRootLoggingLevelIsIgnored()
        {
            Log4Net.Configure()
                .Logging.Root(log => log.At(null))
                .ApplyConfiguration();

            var repo = (Hierarchy)LogManager.GetRepository();
            Assert.That(repo.Root.Level, Is.EqualTo(Level.Debug));
        }

        [Test]
        public void ChildLoggingLevel()
        {
            Log4Net.Configure()
                .Logging.For<Int32>(log => log.At(Level.Warn))
                .Logging.For(typeof(Int64), log => log.At(Level.Notice))
                .Logging.For("Foo", log => log.At(Level.Alert))
                .ApplyConfiguration();

            var intLogger = (Logger)LogManager.GetLogger(typeof(Int32)).Logger;
            var longLogger = (Logger)LogManager.GetLogger(typeof(Int64)).Logger;
            var fooLogger = (Logger)LogManager.GetLogger("Foo").Logger;

            Assert.That(intLogger.Level, Is.EqualTo(Level.Warn));
            Assert.That(longLogger.Level, Is.EqualTo(Level.Notice));
            Assert.That(fooLogger.Level, Is.EqualTo(Level.Alert));
        }

        [Test]
        public void NullChildLoggingLevelAppliesRootLevel()
        {
            Log4Net.Configure()
                .Logging.Root(log => log.At(Level.Trace))
                .Logging.For<Int32>(log => log.At(null))
                .Logging.For(typeof(Int64), log => log.At(null))
                .Logging.For("Foo", log => log.At(null))
                .ApplyConfiguration();

            var intLogger = (Logger)LogManager.GetLogger(typeof(Int32)).Logger;
            var longLogger = (Logger)LogManager.GetLogger(typeof(Int64)).Logger;
            var fooLogger = (Logger)LogManager.GetLogger("Foo").Logger;

            Assert.That(intLogger.Level, Is.Null);
            Assert.That(longLogger.Level, Is.Null);
            Assert.That(fooLogger.Level, Is.Null);

            Assert.That(intLogger.EffectiveLevel, Is.EqualTo(Level.Trace));
            Assert.That(longLogger.EffectiveLevel, Is.EqualTo(Level.Trace));
            Assert.That(fooLogger.EffectiveLevel, Is.EqualTo(Level.Trace));
        }
    }
}