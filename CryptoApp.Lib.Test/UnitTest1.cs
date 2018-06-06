using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace CryptoApp.Lib.Test
{
    [TestClass]
    public class UnitTest1
    {
        private String ReadRes(String name)
        {
            var asm = Assembly.GetAssembly(typeof(UnitTest1));
            var rs = name.Replace("/", ".").Replace("\\", ".").ToUpperInvariant();

            foreach(var avail in asm.GetManifestResourceNames())
            {
                if (avail.ToUpperInvariant().EndsWith(rs))
                {
                    rs = avail;
                    break;
                }
            }
            using(Stream str = asm.GetManifestResourceStream(rs))
            {
                using(StreamReader sr = new StreamReader(str))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            //Given
            var data = ReadRes("resources/kdb.xml");
            var target = new CryptoService();

            //When
            var root = target.Initialize(data);
            Assert.IsNotNull(root);

            var result = target.Find(new Regex("CeframeworkDrupal")).ToList();
            Assert.AreEqual(1, result.Count);



            var results = target.Find(new Regex("Formazione Password")).ToList().First() ;

            Assert.IsNotNull(results);
        }
        [TestMethod]
        public void Encrypt()
        {
            var data = ReadRes("resources/kdb.xml");
            var target = new StringCipher();
            var enc = target.Encrypt(data, "test");
            var dec = target.Decrypt(enc, "test");
            Assert.AreEqual(dec, data);

        }
    }
}
