using System;
using System.IO;
using NUnit.Framework;

namespace Encryption.NitroKey.Test
{
    public class TestBase
    {
        internal string InputFile;
        internal string OutputFile;
        internal string ResultFile;
        internal string RawFile;

        [SetUp]
        public void Setup()
        {
            this.InputFile = Path.GetTempFileName();
            this.OutputFile = Path.GetTempFileName();
            this.ResultFile = Path.GetTempFileName();
            this.RawFile = Path.GetTempFileName();

            var data = Guid.NewGuid().ToByteArray();
            File.WriteAllBytes(this.InputFile, data);
        }


        [TearDown]
        public void TearDown()
        {
            foreach (var file in new[] { this.InputFile, this.OutputFile, this.ResultFile, this.RawFile })
            {
                try
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}