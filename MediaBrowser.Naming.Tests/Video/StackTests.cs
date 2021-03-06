﻿using MediaBrowser.Naming.Common;
using MediaBrowser.Naming.IO;
using MediaBrowser.Naming.Logging;
using MediaBrowser.Naming.Video;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaBrowser.Naming.Tests.Video
{
    [TestClass]
    public class StackTests : BaseVideoTest
    {
        [TestMethod]
        public void TestSimpleStack()
        {
            var files = new[]
            {
                "Bad Boys (2006) part1.mkv",
                "Bad Boys (2006) part2.mkv",
                "Bad Boys (2006) part3.mkv",
                "Bad Boys (2006) part4.mkv",
                "Bad Boys (2006)-trailer.mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(1, result.Stacks.Count);
            TestStackInfo(result.Stacks[0], "Bad Boys (2006)", 4);
        }

        [TestMethod]
        public void TestFalsePositives()
        {
            var files = new[]
            {
                "Bad Boys (2006).mkv",
                "Bad Boys (2007).mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(0, result.Stacks.Count);
        }

        [TestMethod]
        public void TestFalsePositives2()
        {
            var files = new[]
            {
                "Bad Boys 2006.mkv",
                "Bad Boys 2007.mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(0, result.Stacks.Count);
        }

        [TestMethod]
        public void TestFalsePositives3()
        {
            var files = new[]
            {
                "300 (2006).mkv",
                "300 (2007).mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(0, result.Stacks.Count);
        }

        [TestMethod]
        public void TestFalsePositives4()
        {
            var files = new[]
            {
                "300 2006.mkv",
                "300 2007.mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(0, result.Stacks.Count);
        }

        [TestMethod]
        public void TestStackName()
        {
            var files = new[]
            {
                "d:\\movies\\300 2006 part1.mkv",
                "d:\\movies\\300 2006 part2.mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(1, result.Stacks.Count);
            TestStackInfo(result.Stacks[0], "300 2006", 2);
        }

        [TestMethod]
        public void TestDirtyNames()
        {
            var files = new[]
            {
                "Bad Boys (2006).part1.stv.unrated.multi.1080p.bluray.x264-rough.mkv",
                "Bad Boys (2006).part2.stv.unrated.multi.1080p.bluray.x264-rough.mkv",
                "Bad Boys (2006).part3.stv.unrated.multi.1080p.bluray.x264-rough.mkv",
                "Bad Boys (2006).part4.stv.unrated.multi.1080p.bluray.x264-rough.mkv",
                "Bad Boys (2006)-trailer.mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(1, result.Stacks.Count);
            TestStackInfo(result.Stacks[0], "Bad Boys (2006).stv.unrated.multi.1080p.bluray.x264-rough", 4);
        }

        [TestMethod]
        public void TestNumberedFiles()
        {
            var files = new[]
            {
                "Bad Boys (2006).mkv",
                "Bad Boys (2006) 1.mkv",
                "Bad Boys (2006) 2.mkv",
                "Bad Boys (2006) 3.mkv",
                "Bad Boys (2006)-trailer.mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(0, result.Stacks.Count);
        }

        [TestMethod]
        public void TestSimpleStackWithNumericName()
        {
            var files = new[]
            {
                "300 (2006) part1.mkv",
                "300 (2006) part2.mkv",
                "300 (2006) part3.mkv",
                "300 (2006) part4.mkv",
                "300 (2006)-trailer.mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(1, result.Stacks.Count);
            TestStackInfo(result.Stacks[0], "300 (2006)", 4);
        }

        [TestMethod]
        public void TestMixedExpressionsNotAllowed()
        {
            var files = new[]
            {
                "Bad Boys (2006) part1.mkv",
                "Bad Boys (2006) part2.mkv",
                "Bad Boys (2006) part3.mkv",
                "Bad Boys (2006) parta.mkv",
                "Bad Boys (2006)-trailer.mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(1, result.Stacks.Count);
            TestStackInfo(result.Stacks[0], "Bad Boys (2006)", 3);
        }

        [TestMethod]
        public void TestDualStacks()
        {
            var files = new[]
            {
                "Bad Boys (2006) part1.mkv",
                "Bad Boys (2006) part2.mkv",
                "Bad Boys (2006) part3.mkv",
                "Bad Boys (2006) part4.mkv",
                "Bad Boys (2006)-trailer.mkv",
                "300 (2006) part1.mkv",
                "300 (2006) part2.mkv",
                "300 (2006) part3.mkv",
                "300 (2006)-trailer.mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(2, result.Stacks.Count);
            TestStackInfo(result.Stacks[1], "Bad Boys (2006)", 4);
            TestStackInfo(result.Stacks[0], "300 (2006)", 3);
        }

        [TestMethod]
        public void TestDirectories()
        {
            var files = new[]
            {
                "blah blah - cd 1",
                "blah blah - cd 2"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveDirectories(files);

            Assert.AreEqual(1, result.Stacks.Count);
            TestStackInfo(result.Stacks[0], "blah blah", 2);
        }

        [TestMethod]
        public void TestFalsePositive()
        {
            var files = new[]
            {
                "300a.mkv",
                "300b.mkv",
                "300c.mkv",
                "300-trailer.mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(1, result.Stacks.Count);

            TestStackInfo(result.Stacks[0], "300", 3);
        }

        [TestMethod]
        public void TestFailSequence()
        {
            var files = new[]
            {
                "300 part1.mkv",
                "300 part2.mkv",
                "Avatar",
                "Avengers part1.mkv",
                "Avengers part2.mkv",
                "Avengers part3.mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(2, result.Stacks.Count);

            TestStackInfo(result.Stacks[0], "300", 2);
            TestStackInfo(result.Stacks[1], "Avengers", 3);
        }

        [TestMethod]
        public void TestMixedExpressions()
        {
            var files = new[]
            {
                "Bad Boys (2006) part1.mkv",
                "Bad Boys (2006) part2.mkv",
                "Bad Boys (2006) part3.mkv",
                "Bad Boys (2006) part4.mkv",
                "Bad Boys (2006)-trailer.mkv",
                "300 (2006) parta.mkv",
                "300 (2006) partb.mkv",
                "300 (2006) partc.mkv",
                "300 (2006) partd.mkv",
                "300 (2006)-trailer.mkv",
                "300a.mkv",
                "300b.mkv",
                "300c.mkv",
                "300-trailer.mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(3, result.Stacks.Count);

            TestStackInfo(result.Stacks[0], "300 (2006)", 4);
            TestStackInfo(result.Stacks[1], "300", 3);
            TestStackInfo(result.Stacks[2], "Bad Boys (2006)", 4);
        }

        [TestMethod]
        public void TestAlphaLimitOfFour()
        {
            var files = new[]
            {
                "300 (2006) parta.mkv",
                "300 (2006) partb.mkv",
                "300 (2006) partc.mkv",
                "300 (2006) partd.mkv",
                "300 (2006) parte.mkv",
                "300 (2006) partf.mkv",
                "300 (2006) partg.mkv",
                "300 (2006)-trailer.mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(1, result.Stacks.Count);

            TestStackInfo(result.Stacks[0], "300 (2006)", 4);
        }

        [TestMethod]
        public void TestMixed()
        {
            var files = new[]
            {
                new PortableFileInfo{FullName = "Bad Boys (2006) part1.mkv", Type = FileInfoType.File},
                new PortableFileInfo{FullName = "Bad Boys (2006) part2.mkv", Type = FileInfoType.File},
                new PortableFileInfo{FullName = "300 (2006) part2", Type = FileInfoType.Directory},
                new PortableFileInfo{FullName = "300 (2006) part3", Type = FileInfoType.Directory},
                new PortableFileInfo{FullName = "300 (2006) part1", Type = FileInfoType.Directory}
            };

            var resolver = GetResolver();

            var result = resolver.Resolve(files);

            Assert.AreEqual(2, result.Stacks.Count);
            TestStackInfo(result.Stacks[0], "300 (2006)", 3);
            TestStackInfo(result.Stacks[1], "Bad Boys (2006)", 2);
        }

        [TestMethod]
        public void TestDirectories2()
        {
            //TestDirectory(@"blah blah", false, @"blah blah");
            //TestDirectory(@"d:\\music\weezer\\03 Pinkerton", false, "03 Pinkerton");
            //TestDirectory(@"d:\\music\\michael jackson\\Bad (2012 Remaster)", false, "Bad (2012 Remaster)");

            //TestDirectory(@"blah blah - cd1", true, "blah blah");
            //TestDirectory(@"blah blah - disc1", true, "blah blah");
            //TestDirectory(@"blah blah - disk1", true, "blah blah");
            //TestDirectory(@"blah blah - pt1", true, "blah blah");
            //TestDirectory(@"blah blah - part1", true, "blah blah");
            //TestDirectory(@"blah blah - dvd1", true, "blah blah");

            //// Add a space
            //TestDirectory(@"blah blah - cd 1", true, "blah blah");
            //TestDirectory(@"blah blah - disc 1", true, "blah blah");
            //TestDirectory(@"blah blah - disk 1", true, "blah blah");
            //TestDirectory(@"blah blah - pt 1", true, "blah blah");
            //TestDirectory(@"blah blah - part 1", true, "blah blah");
            //TestDirectory(@"blah blah - dvd 1", true, "blah blah");

            //// Not case sensitive
            //TestDirectory(@"blah blah - Disc1", true, "blah blah");
        }

        [TestMethod]
        public void TestNamesWithoutParts()
        {
            // No stacking here because there is no part/disc/etc
            var files = new[]
            {
                "Harry Potter and the Deathly Hallows.mkv",
                "Harry Potter and the Deathly Hallows 1.mkv",
                "Harry Potter and the Deathly Hallows 2.mkv",
                "Harry Potter and the Deathly Hallows 3.mkv",
                "Harry Potter and the Deathly Hallows 4.mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(0, result.Stacks.Count);
        }

        [TestMethod]
        public void TestNumbersAppearingBeforePartNumber()
        {
            // No stacking here because there is no part/disc/etc
            var files = new[]
            {
                "Neverland (2011)[720p][PG][Voted 6.5][Family-Fantasy]part1.mkv",
                "Neverland (2011)[720p][PG][Voted 6.5][Family-Fantasy]part2.mkv"
            };

            var resolver = GetResolver();

            var result = resolver.ResolveFiles(files);

            Assert.AreEqual(1, result.Stacks.Count);
            Assert.AreEqual(2, result.Stacks[0].Files.Count);
        }

        private void TestStackInfo(FileStack stack, string name, int fileCount)
        {
            Assert.AreEqual(fileCount, stack.Files.Count);
            Assert.AreEqual(name, stack.Name);
        }

        private StackResolver GetResolver()
        {
            return new StackResolver(new NamingOptions(), new NullLogger());
        }
    }
}
