using Modulo.Collect.OVAL.Common.comparators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.OVAL.Tests
{


    [TestClass()]
    public class OvalEntityComparerTest
    {

        public OvalEntityComparerTest()
        {
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_compare_two_equal_not_null_entity_values()
        {
            
            Assert.IsTrue(OvalEntityComparer.IsEntityValuesEquals(OperationEnumeration.equals, "aaa", "aaa"));
            Assert.IsFalse(OvalEntityComparer.IsEntityValuesEquals(OperationEnumeration.equals, "aaa", "AAA"));
            Assert.IsTrue(OvalEntityComparer.IsEntityValuesEquals(OperationEnumeration.caseinsensitiveequals, "aaa", "AAA"));
            Assert.IsFalse(OvalEntityComparer.IsEntityValuesEquals(OperationEnumeration.equals, "aaa", "avaa"));
        }


        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_compare_two_not_equal_not_null_entity_values()
        {
            Assert.IsFalse(OvalEntityComparer.IsEntityValuesNotEqual(OperationEnumeration.notequal, "aaa", "aaa"));
            Assert.IsTrue(OvalEntityComparer.IsEntityValuesNotEqual(OperationEnumeration.notequal, "aaa", "AAA"));
            Assert.IsFalse(OvalEntityComparer.IsEntityValuesNotEqual(OperationEnumeration.caseinsensitivenotequal, "aaa", "AAA"));
            Assert.IsTrue(OvalEntityComparer.IsEntityValuesNotEqual(OperationEnumeration.notequal, "aaa", "avaa"));
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_compare_two_entity_values_where_one_is_null_and_the_other_is_not_null()
        {

            Assert.IsFalse(OvalEntityComparer.IsEntityValuesEquals(OperationEnumeration.equals, null, "aaa"));
            Assert.IsFalse(OvalEntityComparer.IsEntityValuesEquals(OperationEnumeration.equals, "aaa", null));

            Assert.IsTrue(OvalEntityComparer.IsEntityValuesNotEqual(OperationEnumeration.notequal, null, "aaa"));
            Assert.IsTrue(OvalEntityComparer.IsEntityValuesNotEqual(OperationEnumeration.notequal, "aaa", null));

        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_compare_two_null_entity_values()
        {
            Assert.IsTrue(OvalEntityComparer.IsEntityValuesEquals(OperationEnumeration.equals, null, null));
            Assert.IsFalse(OvalEntityComparer.IsEntityValuesNotEqual(OperationEnumeration.equals, null, null));
        }


    }
}
