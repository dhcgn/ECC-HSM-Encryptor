using NUnit.Framework;

namespace Contract.Test
{
    [TestFixture]
    public class CurveHelperTest
    {
        [Test]
        public void GetCurveDescriptionFromEcParam()
        {
            var result = CurveHelper.GetCurveDescriptionFromEcParam("06092b2403030208010109");

            Assert.That(result, Contains.Substring("brainpoolP320r1"));
            Assert.That(result, Contains.Substring("320 bit"));
            Assert.That(result, Is.EqualTo("brainpoolP320r1 (320 bit)"));
        }
    }
}