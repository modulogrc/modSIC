/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 *   
 * - Redistributions in binary form must reproduce the above copyright 
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 *   
 * - Neither the name of Modulo Security, LLC nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific  prior written permission.
 *   
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Modulo.Collect.Probe.Common.Test
{
    [TestClass]
    public class CredentialsTest
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_return_empty_string_for_null_credentials_parts()
        {
            var credentials = new Credentials(null, null, null, null);

            Assert.AreEqual(string.Empty, credentials.GetDomain(), "Domain value must be empty.");
            Assert.AreEqual(string.Empty, credentials.GetUserName(), "Username value must be empty.");
            Assert.AreEqual(string.Empty, credentials.GetPassword(), "Password value must be empty."); 
            Assert.AreEqual(string.Empty, credentials.GetAdminPassword(), "Administrative Password value must be empty.");
            Assert.AreEqual(string.Empty, credentials.GetFullyQualifiedUsername(), "Fully qualified username value must be empty.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_be_possible_to_get_all_credential_informations()
        {
            var credentials = new Credentials("mss", "lfernandes", "123456\\b", "RooT");

            AssertCredentials(credentials, "mss", "lfernandes", "RooT");
            Assert.AreEqual("123456\\b", credentials.GetPassword(), "An unexpected password value was found.");
        }

        [TestMethod, Owner("lfernandes")]
        public void If_credentials_was_created_with_no_domain_the_domain_information_must_be_extracted_from_username()
        {
            var credentials = new Credentials(null, @"172.1.1.0\administrator", "1\\2\\3", string.Empty);
            
            AssertCredentials(credentials, "172.1.1.0", "administrator", string.Empty);
        }

        [TestMethod, Owner("lfernandes")]
        public void When_the_domain_is_blank_or_null_the_fully_qualified_username_must_be_the_concatenation_between_dot_slash_and_username()
        {
            {
                var credentialsWithNullDomain = new Credentials(null, "admin", "fakePassword", string.Empty);
                AssertCredentialsWithDotOnDomain(credentialsWithNullDomain, "admin");
            }
            {
                var credentialsWithWhiteSpaceDomain = new Credentials("   ", "admin", "fakePassword", string.Empty);
                AssertCredentialsWithDotOnDomain(credentialsWithWhiteSpaceDomain, "admin");
            }
            {
                var credentialsWithBlankDomain = new Credentials(string.Empty, "admin", "fakePassword", string.Empty);
                AssertCredentialsWithDotOnDomain(credentialsWithBlankDomain, "admin");
            }
        }

        [TestMethod, Owner("lfernandes")]
        public void If_a_username_with_embedded_domain_was_given_the_fully_qualified_username_must_not_contains_the_dot_character_even_when_the_domain_field_is_null()
        {
            var credentialsWithDomainInUsername = new Credentials(string.Empty, @"10.1.0.170\oval", string.Empty, string.Empty);
            AssertCredentials(credentialsWithDomainInUsername, "10.1.0.170", "oval", string.Empty);
        }

        [TestMethod, Owner("lfernandes")]
        public void If_username_was_given_with_embedded_domain_the_domain_field_must_be_ignored()
        {
            var credentialsWithDomainEmbeddedInUsernameAndInDomainField = 
                    new Credentials(".", @"10.1.1.1\oval", string.Empty, string.Empty);

            AssertCredentials(credentialsWithDomainEmbeddedInUsernameAndInDomainField, "10.1.1.1", "oval", string.Empty);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_hold_administrative_password()
        {
            var credentialsSample = new Credentials(null, "oval", "UserP@$$w0rd", "R00t");

            var adminPassword = credentialsSample.GetAdminPassword();

            Assert.AreEqual("R00t", adminPassword);
        }

            

        private void AssertCredentials(Credentials credentialsToAssert, string expectedDomain, string expectedUsername, string expectedAdminPassword)
        {
            var expectedFullyQualifiedName = string.Format(@"{0}\{1}", expectedDomain, expectedUsername);

            Assert.AreEqual(
                expectedDomain, 
                credentialsToAssert.GetDomain(), 
                "An unexpected domain was found.");
            
            Assert.AreEqual(
                expectedUsername, 
                credentialsToAssert.GetUserName(), 
                "An unexpected username was found.");

            Assert.AreEqual(
                expectedFullyQualifiedName, 
                credentialsToAssert.GetFullyQualifiedUsername(), 
                "An unexpected fully qualified username was found.");

            Assert.AreEqual(
                expectedAdminPassword,
                credentialsToAssert.GetAdminPassword(),
                "An unexpected administrative password was found.");
        }

        private void AssertCredentialsWithDotOnDomain(Credentials credentialsToAssert, string expectedUsernameWithoutDot)
        {
            var isDomainNullOrWhiteSpace = string.IsNullOrWhiteSpace(credentialsToAssert.GetDomain());
            Assert.IsTrue(isDomainNullOrWhiteSpace, "The domain in credentials must be blank or null");

            var justUsername = credentialsToAssert.GetUserName();
            Assert.AreEqual(
                expectedUsernameWithoutDot, 
                justUsername, 
                "Unexpected username was found");

            var expectedFullQualifiedUsername = string.Format(@"{0}\{1}", ".", justUsername);
            Assert.AreEqual(
                expectedFullQualifiedUsername, 
                credentialsToAssert.GetFullyQualifiedUsername(), 
                "Unexpected fully qualified username was found.");
        }
    }
}
